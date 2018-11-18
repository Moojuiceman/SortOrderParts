using Harmony12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityModManagerNet;
using Utils;

namespace SortOrderPartsNS
{
    public static class SortOrderParts
    {
        static int currentProfile = 0;
        static List<int> sortedJobs = new List<int>();

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            if (modEntry.Enabled)
            {
                UMMLogger.Init(modEntry.Path + "/Log.txt", "Starting");

                HarmonyInstance harmony = HarmonyInstance.Create(modEntry.Info.Id);

                harmony.PatchAll();

                UMMLogger.WriteLog("Patched");
            }

            return true;
        }

        internal static void SortJob(ref Job job, CarLoader carLoader)
        {
            if (currentProfile != ProfileManager.Get().GetSelectedProfile())
            {
                currentProfile = ProfileManager.Get().GetSelectedProfile();
                sortedJobs.Clear();
            }

            if (!sortedJobs.Contains(job.id))
            {
                foreach (JobPart jobPart in job.jobParts)
                {
                    Dictionary<string, int> partDict = new Dictionary<string, int>();

                    //Store the existing order
                    for (int i = 0; i < jobPart.partsCount; i++)
                    {
                        partDict.Add(jobPart.partList[i], i);
                    }

                    List<string> sorted = new List<string>();

                    //Sort them by the display name
                    if (jobPart.type != "Body")
                    {
                        sorted = (from p in jobPart.partList
                                  orderby GameInventory.Get().GetItemLocalizeName(p.Substring(p.LastIndexOf('/') + 1))
                                  select p).ToList();
                    }
                    else
                    {
                        sorted = (from p in jobPart.partList
                                  orderby GameInventory.Get().GetBodyLocalizedName(carLoader.carToLoad + "-" + carLoader.GetDefaultName(p.Substring(p.LastIndexOf('/') + 1)))
                                  select p).ToList();
                    }

                    jobPart.partList = sorted;
                }

                sortedJobs.Add(job.id);
            }
        }
    }

    [HarmonyPatch(typeof(OrderGenerator), "GetJobForCarLoader")]
    static class OrderGenerator_GetJobForCarLoader_Patch
    {
        static void Postfix(ref Job __result, int carLoaderID)
        {
            try
            {
                SortOrderParts.SortJob(ref __result, CarLoaderPlaces.Get().GetCarLoaderByIndex(carLoaderID));
            }
            catch (Exception e)
            {
                UMMLogger.LogException(e);
            }
        }
    }
}
