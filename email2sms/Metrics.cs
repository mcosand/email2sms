using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace email2sms
{
  public class Metrics
  {
    static Metrics()
    {
      TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];
    }

    public static void Exception(Exception ex)
    {
      File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ex.log"), ex.ToString() + "\r\n");
      Trace.TraceError(ex.ToString());
      var telemetry = new TelemetryClient();
      telemetry.TrackException(ex);
    }

    public static void Error(string msg)
    {
      File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ex.log"), msg + "\r\n");
      Trace.TraceError(msg);
      var telemetry = new TelemetryClient();
      telemetry.TrackEvent($"ERROR {msg}");
    }

    public static void Info(string msg)
    {
      File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ex.log"), msg + "\r\n");
      Trace.TraceInformation(msg);
      var telemetry = new TelemetryClient();
      telemetry.TrackEvent($"INFO {msg}");
    }
  }
}