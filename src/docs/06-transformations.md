# 6. DataWeave Transformations

The legacy flows perform a small number of DataWeave 2.0 transformations.
This document lists each one, its location, its input/output, and its
behaviour.

## 6.1 `GetMovies` — JDBC rows → JSON

Source — `mulesoft/src/main/mule/implementation.xml:14-21`.

```dwl
%dw 2.0
output application/json
---
payload
```

| Aspect         | Value                                                              |
|----------------|--------------------------------------------------------------------|
| Input          | Java `List<Map<String, Object>>` from the Mule DB connector        |
| Output         | `application/json` (array of objects)                              |
| Logic          | Identity — every input row/column is emitted verbatim              |
| Renames/filters| None                                                               |

There is no projection, no derived field, no conditional logic.

## 6.2 `BookTickets` — variable assembly for inputs

Source — `mulesoft/src/main/mule/implementation.xml:25` (a `<set-variable>`
expression, not a `<ee:transform>`):

```dwl
{
  no_tickets: attributes.queryParams.no_tickets,
  m_id:       attributes.uriParams.m_id
}
```

| Aspect | Value                                                                                  |
|--------|----------------------------------------------------------------------------------------|
| Input  | HTTP request attributes from the Mule HTTP listener                                    |
| Output | A Mule variable named `no_tickets` (confusing — it is an *object*, not the count!)     |
| Stored | `vars.no_tickets.no_tickets` = the count (string), `vars.no_tickets.m_id` = the movie id (string) |

⚠️ Naming hazard: the variable is called `no_tickets` but is actually an
object containing **both** `no_tickets` and `m_id`. Throughout the flow
the count is accessed as `vars.no_tickets.no_tickets`. Migration code
should rename this for clarity.

## 6.3 `BookTickets` — input parameter expressions for `INSERT`

Source — `mulesoft/src/main/mule/implementation.xml:35-41`:

```dwl
{
  'm_id':       vars.no_tickets.m_id        as Number,
  'no_tickets': vars.no_tickets.no_tickets  as Number,
  'price':
      if (vars.no_tickets.no_tickets as Number <= 5)  vars.no_tickets.no_tickets as Number * 100
      else if (vars.no_tickets.no_tickets as Number <= 10) vars.no_tickets.no_tickets as Number * 90
      else vars.no_tickets.no_tickets as Number * 80
}
```

This is where the [pricing tiers](./03-business-rules.md#34-pricing-tiers)
are encoded. The DataWeave expression both **coerces** the string inputs
to numbers and **computes** the price from the count.

## 6.4 `BookTickets` — input parameter expression for `UPDATE`

Source — `mulesoft/src/main/mule/implementation.xml:45-48`:

```dwl
{
  'm_id':       vars.no_tickets.m_id        as Number,
  'no_tickets': vars.no_tickets.no_tickets  as Number
}
```

Identical to §6.3 except the `price` is not needed for the `UPDATE` —
which only decrements `m_available`.

## 6.5 `BookTickets` — JDBC rows → JSON (confirmation body)

Source — `mulesoft/src/main/mule/implementation.xml:55-61`.

```dwl
%dw 2.0
output application/json
---
payload
```

Identical structure to §6.1: identity transform of the newest order row.

## 6.6 `BookTickets` — error path: JDBC rows → Java

Source — `mulesoft/src/main/mule/implementation.xml:64-71` (inside the
`VALIDATION:INVALID_BOOLEAN` handler):

```dwl
%dw 2.0
output application/java
---
payload
```

| Aspect | Value                                                                |
|--------|----------------------------------------------------------------------|
| Input  | The payload at the moment of the error — which is the result of the `SELECT m_available …` |
| Output | The same data, re-typed as `application/java`                        |
| Why    | Subsequent string-interpolation in the next `<set-payload>` needs the value to be a Java object indexable by `[0].m_available` |

## 6.7 `BookTickets` — error path: rejection message

Source — `mulesoft/src/main/mule/implementation.xml:72`:

```dwl
%dw 2.0
---
{
  "error": "avaible tickets is only $(payload[0].m_available) but you have ordered $(vars.no_tickets.no_tickets)"
}
```

Produces the JSON body returned when a booking is rejected. The output
mime type defaults to JSON via the outer `<set-payload>` (`output
application/json` is implicit in the `value` expression of the
`set-payload`). Typos (`avaible`) are preserved.

## 6.8 APIKit error-handler transforms (`interface.xml`)

The `movie-main` flow defines one DataWeave message per APIKit error type.
All are static one-liners:

| Error type                          | DataWeave body                          | HTTP status |
|-------------------------------------|------------------------------------------|-------------|
| `APIKIT:BAD_REQUEST`                | `{ message: "Bad request" }`            | 400         |
| `APIKIT:NOT_FOUND`                  | `{ message: "Resource not found" }`     | 404         |
| `APIKIT:METHOD_NOT_ALLOWED`         | `{ message: "Method not allowed" }`     | 405         |
| `APIKIT:NOT_ACCEPTABLE`             | `{ message: "Not acceptable" }`         | 406         |
| `APIKIT:UNSUPPORTED_MEDIA_TYPE`     | `{ message: "Unsupported media type" }` | 415         |
| `APIKIT:NOT_IMPLEMENTED`            | `{ message: "Not Implemented" }`        | 501         |

Each handler sets `vars.httpStatus` to the corresponding code, which the
HTTP listener then uses on the outbound response (see
`07-error-handling.md`).
