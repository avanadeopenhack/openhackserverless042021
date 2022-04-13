# openhackserverless042021

## Challenge 4 System telemetry and API Management

### Telemetry

The number of times each function was run in the past hour and the average duration of each function in that hour
``` kusto
requests
| where timestamp > ago(1h) and operation_Name contains "Rating"
| summarize totalCount=sum(itemCount), avgDuration = avg(duration) by operation_Name
| render table
```

### APIM

Three products:
- Mobile
- Internal
- External

|API|Operations\Products|Mobile|Internal|External|
|---|---|---|---|---|
|-|GetUsers|-|-|-|
|UserReadAPI|GetUsers|X|-|-|
|ProductReadAPI|GetProduct|X|X|X|
|ProductReadAPI|GetProducts|X|X|X|
|RatingWriteAPI|CreateRating|X|-|-|
|RatingReadAPI|GetRating|X|X|-|
|RatingReadAPI|GetRatings|X|X|-|
| | | | | |
|Policies|
|10 calls per 60 seconds| |-|X|-|
|15 calls per 60 seconds| |-|-|X|


