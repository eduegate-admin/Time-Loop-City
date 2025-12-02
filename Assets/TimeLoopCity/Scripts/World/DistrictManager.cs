using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.World
{
    public enum DistrictType
    {
        FortKochi,
        MarineDrive,
        WillingdonIsland,
        Mattancherry,
        Edappally,
        Generic
    }

    public static class DistrictManager
    {
        public static DistrictType GetDistrictAtPosition(Vector3 position)
        {
            // Simple bounding box logic for now
            // In a real OSM import, we'd use polygon bounds
            
            if (position.x < -100) return DistrictType.FortKochi;
            if (position.x > 100 && position.z < 0) return DistrictType.WillingdonIsland;
            if (position.x > 50 && position.z > 50) return DistrictType.Edappally;
            if (position.x < -80 && position.z < -50) return DistrictType.Mattancherry;
            if (position.x > -50 && position.x < 50 && position.z > -20 && position.z < 60) return DistrictType.MarineDrive;
            
            return DistrictType.Generic;
        }

        public static ProceduralBuildingArchitect.BuildingStyle GetStyleForDistrict(DistrictType district)
        {
            switch (district)
            {
                case DistrictType.FortKochi: return ProceduralBuildingArchitect.BuildingStyle.Colonial;
                case DistrictType.MarineDrive: return ProceduralBuildingArchitect.BuildingStyle.Modern;
                case DistrictType.WillingdonIsland: return ProceduralBuildingArchitect.BuildingStyle.Modern; // Industrial
                case DistrictType.Mattancherry: return ProceduralBuildingArchitect.BuildingStyle.Slum; // Dense/Old
                case DistrictType.Edappally: return ProceduralBuildingArchitect.BuildingStyle.Modern;
                default: return ProceduralBuildingArchitect.BuildingStyle.Modern;
            }
        }
        
        public static float GetBuildingHeightForDistrict(DistrictType district)
        {
            switch (district)
            {
                case DistrictType.MarineDrive: return Random.Range(20f, 60f); // High rises
                case DistrictType.Edappally: return Random.Range(15f, 40f);
                case DistrictType.FortKochi: return Random.Range(6f, 12f); // Low rise
                case DistrictType.Mattancherry: return Random.Range(5f, 10f);
                case DistrictType.WillingdonIsland: return Random.Range(8f, 20f);
                default: return Random.Range(10f, 25f);
            }
        }
    }
}
