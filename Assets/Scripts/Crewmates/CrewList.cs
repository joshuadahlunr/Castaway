using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crew;
using Extensions;
using SQLite;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Crew.Globals {
    /// <summary>
    /// Global list of all crew members that exist
    /// </summary>
    /// <author>Misha Desear</author>
    public class CrewList : Global {
        public class GlobalCrewList 
        {
                [PrimaryKey, Unique, AutoIncrement]
                public int id { set; get; }
                public string name { set; get; }
        }

        public class CrewListMember
        {
                [PrimaryKey, Unique, AutoIncrement]
                public int id { set; get; }

                [NotNull]
                public int listID { set; get; }

                [NotNull]
                public string name { set; get; }
        }

        public Crewmates[] injectCrewProtos;

        public CrewDatabase crewDB;

    #if UNITY_EDITOR
        public void Update() {
            if (injectCrewProtos == null) return;

            foreach(var crewProto in injectCrewProtos)
                AddCrewMember(Instantiate(crewProto), 0);

            injectCrewProtos = null;
        }
    #endif

            /// <exception cref="ArgumentException"></exception>
        public void DatabaseLoad(string name = "Global Crew", bool clear = true) 
        {
                var crewList = DatabaseManager.GetOrCreateTable<GlobalCrewList>().FirstOrDefault(l => l.name == name);
                if (crewList is null)
                    throw new ArgumentException($"The crewlist {name} could not be found in the database!");

                DatabaseLoad(crewList.id, clear);
        }

        public virtual void DatabaseLoad(int crewlistID, bool clear = true) 
        {
                var crewMembers = DatabaseManager.GetOrCreateTable<CrewListMember>().Where(crewMember => crewMember.listID == crewlistID);

                if (clear) RemoveAllCrewmates();

                foreach (var crewProto in injectCrewProtos) 
                    AddCrewMember(Instantiate(crewProto), 0);
                injectCrewProtos = null;

                foreach(var crewmates in crewmates) 
                {
                    var instantiated = crewDB.Instantiate(crewmates.name);
                    AddCrewMember(instantiated);
                }
        }
    }
}