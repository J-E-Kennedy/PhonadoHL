using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Phonado.Logging;
using Phonado.PhotoEffects;
using UnityEngine;

public static class Logger
{
   //intended to check name of image against imageplanes time held
   public static Dictionary<string, SessionPhotoLogDto> IndividualImageHeldTimes = new Dictionary<string, SessionPhotoLogDto>();
   public static List<Vector3> LookPoints;
   public static float LookRecordRate = 1f;

   public static int ExperienceID { get; set; }
   public static DateTime StartTime { get; set; }
   
   static HttpClient client = new HttpClient();
   
   public static DateTime EndTime { get; set; }
   public static string Location { get; set; }
   public static string EndReason { get; set; }
   public static int UserID { get; set; }
   public static BallLogDto Ball { get; set; }
   public static ICollection<EyeTrackingLogDto> EyeTrackings { get; set; }
   public static ICollection<GrabLogDto> Grabs { get; set; }
   public static ICollection<SessionPhotoLogDto> SessionPhotos {get; set; }
   public static ICollection<PositionLogDto> Positions { get; set; }

   public static async void writeToFile()
   {

      var log = new
      {
         ExperienceID,
         StartTime,
         EndTime,
         Location,
         EndReason,
         UserID,
         Ball,
         EyeTrackings,
         Grabs,
         SessionPhotos,
         Positions
      };

      StringBuilder sb = new StringBuilder();
      sb.AppendLine("{");
      sb.AppendLine($"  \"experienceID\": {ExperienceID},");
      sb.AppendLine($"  \"startTime\": \"{StartTime.ToString("O")}\",");
      sb.AppendLine($"  \"endTime\": \"{EndTime.ToString("O")}\",");
      sb.AppendLine($"  \"location\": \"{Location}\",");
      sb.AppendLine($"  \"endReason\": \"{EndReason}\",");
      sb.AppendLine($"  \"userID\": {UserID},");
      sb.AppendLine("  \"ball\": {");
      sb.AppendLine($"    \"xLaunch\": {Ball.XLaunch},");
      sb.AppendLine($"    \"yLaunch\": {Ball.YLaunch},");
      sb.AppendLine($"    \"zLaunch\": {Ball.ZLaunch},");
      sb.AppendLine($"    \"xLand\": {Ball.XLand},");
      sb.AppendLine($"    \"yLand\": {Ball.YLand},");
      sb.AppendLine($"    \"zLand\": {Ball.ZLand},");
      sb.AppendLine($"    \"launchTime\": \"{Ball.LaunchTime.ToString("O")}\",");
      sb.AppendLine($"    \"landTime\": \"{Ball.LandTime.ToString("O")}\"");
      sb.AppendLine("  },");
      sb.AppendLine("  \"eyeTrackings\": [");

      bool first = true;
      foreach (var eye in EyeTrackings)
      {
         if (!first)
         {
            sb.AppendLine(",");
         }
         sb.AppendLine("    {");
         sb.AppendLine($"      \"x\": {eye.X},");
         sb.AppendLine($"      \"y\": {eye.Y},");
         sb.AppendLine($"      \"z\": {eye.Z},");
         sb.AppendLine($"      \"time\": \"{eye.Time.ToString("O")}\"");
         sb.Append("    }");
         first = false;
      }

      if (EyeTrackings.Any())
      {
         sb.AppendLine();
      }
      sb.AppendLine("  ],");
      sb.AppendLine("  \"grabs\": [");
      //do grabs
      first = true;
      foreach (var grab in Grabs)
      {
         if (!first)
         {
            sb.AppendLine(",");
         }
         sb.AppendLine("    {");
         sb.AppendLine($"      \"hand\": \"{grab.Hand}\",");
         sb.AppendLine($"      \"x\": {grab.X},");
         sb.AppendLine($"      \"y\": {grab.Y},");
         sb.AppendLine($"      \"z\": {grab.Z},");
         sb.AppendLine($"      \"confidence\": {grab.Confidence},");
         sb.AppendLine($"      \"time\": \"{grab.Time.ToString("O")}\",");
         sb.AppendLine("      \"position\": {");
         sb.AppendLine($"        \"x\": {grab.Position.X},");
         sb.AppendLine($"        \"y\": {grab.Position.Y},");
         sb.AppendLine($"        \"z\": {grab.Position.Z},");
         sb.AppendLine($"        \"time\": \"{grab.Position.Time}\"");
         sb.AppendLine("      }");
         sb.Append("    }");
         first = false;
      }

      if (Grabs.Any())
      {
         sb.AppendLine();
      }

      sb.AppendLine("  ],");
      sb.AppendLine("  \"sessionPhotos\": [");
      
      //do session photos
      first = true;
      foreach (var photo in SessionPhotos)
      {
         if (!first)
         {
            sb.AppendLine(",");
         }
         sb.AppendLine("    {");
         sb.AppendLine($"      \"photoName\": \"{photo.PhotoName}\",");
         sb.AppendLine($"      \"timesGrabbed\": {photo.TimesGrabbed},");
         sb.AppendLine($"      \"secondsGrabbed\": {photo.SecondsGrabbed}");
         sb.Append("    }");
         first = false;
      }

      if (SessionPhotos.Any())
      {
         sb.AppendLine();
      }
      
      sb.AppendLine("  ],");
      sb.AppendLine("  \"positions\": [");
      
      //do positions
      first = true;
      foreach (var position in Positions)
      {
         if (!first)
         {
            sb.AppendLine(",");
         }
         sb.AppendLine("    {");
         sb.AppendLine($"      \"x\": {position.X},");
         sb.AppendLine($"      \"y\": {position.Y},");
         sb.AppendLine($"      \"z\": {position.Z},");
         sb.AppendLine($"      \"time\": \"{position.Time.ToString("O")}\"");
         sb.Append("    }");
         first = false;
      }

      if (Positions.Any())
      {
         sb.AppendLine();
      }
      
      sb.AppendLine("  ]");
      sb.AppendLine("}");

      
      var response = client.GetAsync(@"https://phonadodb.azurewebsites.net/api/test").Result;
      if (response.IsSuccessStatusCode)
      {
         Debug.Log(sb.ToString());
         //SimpleJSON.JSONObject.
         //var jsonString = JsonConvert.SerializeObject(log);
         //.Log(jsonString);
      }
      else
      {
         Debug.Log("Goodbye World!");
      }
   }
}
