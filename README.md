
# CurrencyConversion Assignment

This assignment is developed by Usman Farooq, and it impelements the functionality required for the skill test.

Selected framework:
- .Net Core 8 Web API (C#)


## Getting Started
```
  Select 'BambooCard.CurrencyConversion.API' as the startup project
```

```
  Build the solution from Visual Studio, or using command: 'dotnet build'
```

```
  Run the solution
```


## Endpoints Reference

#### Get Rates

```http
  GET /api/v1.0/currency-converter/{SOURCE_CURRENCY}/
```

This endpoint will give you the latest rates of the provided currency. 

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `SOURCE_CURRENCY` | `string` | **Required**. Base currency |

#### Convert Currency

```http
  GET /api/v1.0/currency-converter/convert?from={SOURCE}&to={DESTINATION}&amount={AMOUNT}
```

This endpoint will convert the specified amount of the provided Source Currency to Destination Currency

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `SOURCE`      | `string` | **Required**. Source currency ISO-3 code |
| `DESTINATION`      | `string` | **Required**. Destination currency ISO-3 code |
| `AMOUNT`      | `string` | **Required**. Amount to convert |

#### Get Historical Data

``` http
  GET /api/v1.0/currency-converter/{BASE_CURRENCY}/historical?startDate={SDATE}&endDate={EDATE}&pageNumber={PAGE_NUMBER}
```
This endpoint will give you the historical data for the provided BASE_CURRENCY for the provided duration.

This will return the Paged data based on the 'Date', and you can get the next date's data by providing the respective page #.

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `BASE_CURRENCY`      | `string` | **Required**. Base currency ISO-3 code |
| `SDATE`      | `date` | **Required**. Start date in 'yyyy-MM-dd |
| `EDATE`      | `date` | **Required**. End date in 'yyyy-MM-dd |
| `PAGE_NUMBER`      | `int` | **Not Required**. Page number of the data |

## Getting Started
```
  Select 'BambooCard.CurrencyConversion.API' as the startup project
```

```
  Build the solution from Visual Studio, or using command: 'dotnet build'
```

```
  Run the solution
```

