# 5. External Integrations and Configuration

## 5.1 External systems

The legacy application talks to exactly one external system at runtime:

| System | Protocol | Used for | Source |
|--------|----------|----------|--------|
| MySQL on **remotemysql.com** | TCP / MySQL wire | All persistence — read movies, validate availability, write orders, update availability, read newest order | `global.xml`, `config.yaml`, `implementation.xml` |

In addition, at **build time** APIKit pulls the RAML contract from
**Anypoint Exchange**:

| System | Protocol | Used for | Source |
|--------|----------|----------|--------|
| Anypoint Exchange (`maven.anypoint.mulesoft.com`) | HTTPS / Maven | Resolves `dd352549-b3e6-4dd6-b86f-85ed018825af:movie:1.0.0:raml:zip` (the RAML used by `apikit:config`) | `pom.xml:117-122, 145-156` |

And, optionally, at **deploy time** the `mule-maven-plugin` deploys the
built application to **CloudHub** under
`https://anypoint.mulesoft.com` (`pom.xml:33-46`).

There are no other outbound integrations — no message brokers, no
notification/email service, no payment gateway, no customer service.

## 5.2 Mule connectors and global configurations

Source — `mulesoft/src/main/mule/global.xml`:

```xml
<http:listener-config name="movie-httpListenerConfig">
  <http:listener-connection host="0.0.0.0" port="${http.port}" />
</http:listener-config>

<db:config name="Database_Config">
  <db:my-sql-connection host="${db.host}" port="${db.port}"
                        user="${db.user}" password="${db.pass}"
                        database="${db.database}" />
</db:config>

<configuration-properties file="config.yaml"/>
```

Source — `mulesoft/src/main/mule/interface.xml`:

```xml
<apikit:config name="movie-config"
               api="resource::dd352549-b3e6-4dd6-b86f-85ed018825af:movie:1.0.0:raml:zip:movie.raml"
               outboundHeadersMapName="outboundHeaders"
               httpStatusVarName="httpStatus" />
```

| Configuration name        | Type                | Purpose                              |
|---------------------------|---------------------|--------------------------------------|
| `movie-httpListenerConfig`| `http:listener-config` | Inbound HTTP on `0.0.0.0:${http.port}` |
| `Database_Config`         | `db:config` (MySQL) | All SQL access                       |
| `movie-config`            | `apikit:config`     | RAML routing, outbound headers, HTTP-status variable |

## 5.3 Configuration properties (`config.yaml`)

Source — `mulesoft/src/main/resources/config.yaml`:

```yaml
http:
  port: '8081'
db:
  port:     '3306'
  host:     'remotemysql.com'
  user:     'Jk8nbsjqRg'
  pass:     'OwYpM3r0Rx'
  database: 'Jk8nbsjqRg'
```

Resolved property usages:

| Property      | Resolved from | Used by                          |
|---------------|---------------|----------------------------------|
| `http.port`   | `config.yaml` | `<http:listener-connection port=…>` |
| `db.host`     | `config.yaml` | `<db:my-sql-connection host=…>`     |
| `db.port`     | `config.yaml` | `<db:my-sql-connection port=…>`     |
| `db.user`     | `config.yaml` | `<db:my-sql-connection user=…>`     |
| `db.pass`     | `config.yaml` | `<db:my-sql-connection password=…>` |
| `db.database` | `config.yaml` | `<db:my-sql-connection database=…>` |

> ⚠️ Secret-handling defect (legacy): database credentials are committed
> in plaintext to `config.yaml`. There is no use of Mule Secure Properties,
> Anypoint Vault, or environment variables for the production values. The
> migration must:
>
> 1. Treat the committed credentials as compromised and rotate them.
> 2. Store the new credentials in a secrets manager (Azure Key Vault,
>    GitHub Actions secrets, or environment variables in the target
>    runtime).
> 3. Remove the plaintext values from any new config files.

## 5.4 Build dependencies that affect runtime behaviour

From `mulesoft/pom.xml`:

| GroupId : ArtifactId | Version | Why it matters |
|----------------------|---------|----------------|
| `org.mule.connectors:mule-http-connector`   | (Mule 4.4 default) | HTTP listener used by all inbound traffic |
| `org.mule.connectors:mule-db-connector`     | `1.10.4` | All MySQL access |
| `mysql:mysql-connector-java`                | `5.1.48` | JDBC driver used by the DB connector — **old**; migration should pick a modern driver |
| `org.mule.modules:mule-validation-module`   | `1.4.5`  | `validation:is-true` for the availability rule |
| `dd352549-b3e6-4dd6-b86f-85ed018825af:movie`| `1.0.0`  | RAML contract resolved at build time |
| `com.mulesoft.munit:*`                      | `2.3.6`  | Test runner (no tests committed) |
| `org.mule.weave:assertions`                 | `1.0.2`  | Test-only DataWeave assertions |

## 5.5 Deployment configuration

Source — `mulesoft/pom.xml:33-46`:

```xml
<cloudHubDeployment>
  <uri>https://anypoint.mulesoft.com</uri>
  <muleVersion>${mule.version}</muleVersion>
  <username>${anypoint.username}</username>
  <password>${anypoint.password}</password>
  <environment>${env}</environment>
  <applicationName>${appname}</applicationName>
  <businessGroup>${business}</businessGroup>
  <workerType>${vCore}</workerType>
  <workers>${workers}</workers>
  <objectStoreV2>true</objectStoreV2>
</cloudHubDeployment>
```

The deployment targets are parameterised via Maven properties (not
committed). `objectStoreV2` is enabled but no `ObjectStore` connector is
referenced in source — i.e. it is configured but unused.
