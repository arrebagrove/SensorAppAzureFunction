﻿#r "Newtonsoft.Json"

using System;
using System.Configuration;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Net;
using System.Text;

public static HttpResponseMessage Run(HttpRequestMessage req, TraceWriter log)
{
    using (var db = new SensorReadingContext())
    {
        IQueryable<SensorReading> myQuery;
        
        myQuery = db.SensorReadings
            .Where(rr => rr.SensorValue != null)
            .GroupBy(s => s.SensorType)
            .Select(r => r.OrderByDescending(d => d.OutputTime)                    
            .FirstOrDefault());

        foreach (SensorReading s in myQuery)
        {
            log.Info(s.SensorType + " : " + s.SensorValue);            
        }

        var result = JsonConvert.SerializeObject(myQuery);

        var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(result, Encoding.UTF8, "application/json")
            };
        return response;
    }

}

[Table("SensorReading")]
public class SensorReading
{
    public int Id { get; set; }
    public string DeviceId { get; set; }
    public string SensorType { get; set; }
    public string SensorValue { get; set; }
    public DateTime? OutputTime { get; set; }
}

public class SensorReadingContext : DbContext
{
    public SensorReadingContext()
        : base("name=SensorReadingContext")
    {
    }

    public virtual DbSet<SensorReading> SensorReadings { get; set; }
}