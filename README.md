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

