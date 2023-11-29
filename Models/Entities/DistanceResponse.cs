using System;
using System.Collections.Generic;

namespace ApiCos.Models.Entities
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Admin
    {
        public string iso_3166_1_alpha3 { get; set; }
        public string iso_3166_1 { get; set; }
    }

    public class DistanceResponse
    {
        public List<Route> routes { get; set; }
        public List<Waypoint> waypoints { get; set; }
        public string code { get; set; }
        public string uuid { get; set; }
    }

    public class Geometry
    {
        public List<List<double>> coordinates { get; set; }
        public string type { get; set; }
    }

    public class Intersection
    {
        public List<bool?> entry { get; set; }
        public List<int?> bearings { get; set; }
        public double? duration { get; set; }
        public MapboxStreetsV8 mapbox_streets_v8 { get; set; }
        public bool? is_urban { get; set; }
        public int? admin_index { get; set; }
        public int? @out { get; set; }
        public double? weight { get; set; }
        public int? geometry_index { get; set; }
        public List<double?> location { get; set; }
        public int? @in { get; set; }
        public double? turn_weight { get; set; }
        public double? turn_duration { get; set; }
        public List<string> classes { get; set; }
        public TollCollection toll_collection { get; set; }
        public List<Lane> lanes { get; set; }
        public bool? yield_sign { get; set; }
        public bool? traffic_signal { get; set; }
        public string tunnel_name { get; set; }
    }

    public class Lane
    {
        public List<string> indications { get; set; }
        public string valid_indication { get; set; }
        public bool? valid { get; set; }
        public bool? active { get; set; }
    }

    public class Leg
    {
        public List<object> via_waypoints { get; set; }
        public List<Admin> admins { get; set; }
        public double? weight { get; set; }
        public double? duration { get; set; }
        public List<Step> steps { get; set; }
        public double? distance { get; set; }
        public string summary { get; set; }
    }

    public class Maneuver
    {
        public string type { get; set; }
        public string instruction { get; set; }
        public int? bearing_after { get; set; }
        public int? bearing_before { get; set; }
        public List<double?> location { get; set; }
        public string modifier { get; set; }
        public int? exit { get; set; }
    }

    public class MapboxStreetsV8
    {
        public string @class { get; set; }
    }

    public class Root
    {
        public DistanceResponse DistanceResponse { get; set; }
    }

    public class Route
    {
        public string weight_name { get; set; }
        public double? weight { get; set; }
        public double? duration { get; set; }
        public double? distance { get; set; }
        public List<Leg> legs { get; set; }
        public Geometry geometry { get; set; }
    }

    public class Step
    {
        public List<Intersection> intersections { get; set; }
        public Maneuver maneuver { get; set; }
        public string name { get; set; }
        public double? duration { get; set; }
        public double? distance { get; set; }
        public string driving_side { get; set; }
        public double? weight { get; set; }
        public string mode { get; set; }
        public Geometry geometry { get; set; }
        public string @ref { get; set; }
        public string destinations { get; set; }
        public string rotary_name { get; set; }
    }

    public class TollCollection
    {
        public string name { get; set; }
        public string type { get; set; }
    }

    public class Waypoint
    {
        public double? distance { get; set; }
        public string name { get; set; }
        public List<double?> location { get; set; }
    }


}
