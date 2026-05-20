# 4. Data Model

The legacy application uses a single MySQL database accessed through the
Mule DB connector (`db:my-sql-connection` in `global.xml`). The database
schema is **not** committed to source — only its column usage in the Mule
flows reveals the shape. This document records what can be inferred and
clearly marks the gaps.

## 4.1 Connection

Source — `mulesoft/src/main/mule/global.xml:11-13` and `config.yaml`:

```yaml
db:
  host:     remotemysql.com
  port:     3306
  database: Jk8nbsjqRg
  user:     Jk8nbsjqRg
  password: OwYpM3r0Rx
```

> ⚠️ The legacy `config.yaml` ships a real-looking host, user, password, and
> database name *in plaintext, in the repository*. The migration must store
> these as secrets (e.g. Azure Key Vault, environment variables) and rotate
> the credentials. See `08-migration-notes.md`.

MySQL connector version: `mysql-connector-java:5.1.48`
(`mulesoft/pom.xml:106-109`). The Mule DB connector is `1.10.4`
(`mulesoft/pom.xml:99-104`).

## 4.2 Tables in use

The flows reference exactly two tables: `movie_table` and `order_table`.

### `movie_table`

Inferred columns:

| Column        | Type (inferred)         | Source / evidence |
|---------------|-------------------------|-------------------|
| `m_id`        | numeric (string-in API) | `WHERE m_id = :m_id` (`implementation.xml:27, 44`); coerced via `as Number` |
| `m_available` | integer ≥ 0             | `WHERE m_available > 0` (`implementation.xml:12`); used in `m_available - :no_tickets` arithmetic (`implementation.xml:32, 44`) |
| _(other)_     | _unknown_               | `SELECT *` is used in `GetMovies`, so additional columns (likely `m_name`, possibly `m_price`, `m_date`, `m_show_time`, etc.) are exposed to clients without being named in source. The auto-generated type hint in `application-types.xml:4-13` mentions `id`, `name`, `no_of_tickets` but this is generated from a single example, not the schema. |

Operations performed on this table:

- `SELECT * FROM movie_table WHERE m_available > 0`
- `SELECT m_available FROM movie_table WHERE m_id = :m_id`
- `UPDATE movie_table SET m_available = m_available - :no_tickets WHERE m_id = :m_id`

### `order_table`

Inferred columns:

| Column        | Type (inferred)              | Source / evidence |
|---------------|------------------------------|-------------------|
| `o_id`        | numeric, monotonic (likely auto-increment / primary key) | `SELECT * FROM order_table WHERE o_id = (SELECT MAX(o_id) FROM order_table)` (`implementation.xml:52`) — no explicit insert into `o_id` means the DB generates it. |
| `m_id`        | numeric                      | `INSERT INTO order_table (m_id, no_tickets, price) VALUES (...)` (`implementation.xml:34`) — value taken from `vars.no_tickets.m_id as Number`. No `FOREIGN KEY` is enforced in source, but the column logically references `movie_table.m_id`. |
| `no_tickets`  | numeric                      | Same insert; value is `vars.no_tickets.no_tickets as Number`. |
| `price`       | numeric                      | Same insert; value is computed per the pricing tiers in `03-business-rules.md`. |

Operations performed on this table:

- `INSERT INTO order_table (m_id, no_tickets, price) VALUES (:m_id, :no_tickets, :price)`
- `SELECT * FROM order_table WHERE o_id = (SELECT MAX(o_id) FROM order_table)`

There is **no** delete, update, or status column on orders. Once written,
an order is immutable from this application's perspective.

## 4.3 Inferred SQL DDL (illustrative only)

The repository contains no `CREATE TABLE` statements. The following is a
**best-effort reconstruction**, suitable as a starting point for the
`src/sql` migration but **not authoritative**:

```sql
CREATE TABLE movie_table (
  m_id        INT          NOT NULL PRIMARY KEY,
  m_name      VARCHAR(255) NULL,        -- inferred from RAML type hint
  m_available INT          NOT NULL,
  -- other columns: unknown (SELECT * exposes them)
);

CREATE TABLE order_table (
  o_id       INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  m_id       INT NOT NULL,
  no_tickets INT NOT NULL,
  price      INT NOT NULL,
  -- no created_at / customer / status columns in source
  CONSTRAINT fk_order_movie FOREIGN KEY (m_id) REFERENCES movie_table (m_id)
);
```

Before migration:

- The migration team should connect to the legacy `remotemysql.com`
  database (with rotated credentials) and dump the real schema. See
  `08-migration-notes.md`.

## 4.4 Concurrency and integrity

- No `BEGIN/COMMIT` is issued by the flows. Each `db:*` operator runs in
  its own connection autocommit. Sequence integrity (insert order → update
  availability → read order) is therefore best-effort.
- No row-level lock is taken on `movie_table` before the
  availability check, enabling oversell under concurrency
  (see `03-business-rules.md` §3.3).
- `SELECT * ... WHERE o_id = MAX(o_id)` is not safe under concurrent
  inserts — it returns the *globally* newest order, not the one this flow
  inserted.

These are real defects of the legacy system. The migration target should
fix them; this document captures them so the new implementation can be
*compatible* with the contract while also *correct*.
