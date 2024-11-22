using System;

namespace HavenZoneCreator.Utilities;

public enum EFlareTypes
{
    none, 
    Light, 
    Airdrop, 
    ExitActivate, 
    Quest, 
    AIFollowEvent
}

public enum EZoneTypes
{
    placeitem, 
    visit, 
    flarezone, 
    botkillzone
}

public enum ELocations
{
    Interchange,
    Factory4_Day,
    Factory4_Night,
    BigMap, // Customs
    Woods,
    Lighthouse,
    Shoreline,
    RezervBase,
    Laboratory,
    TarkovStreets,
    Sandbox, // Ground Zero PMC Level < 20
    Sandbox_High // Ground Zero PMC Level > 20
}