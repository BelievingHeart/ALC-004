using System;
using System.Collections.Generic;
using System.Linq;
using Core.ViewModels.Fai;
using MeasurementCommon.Fai;

namespace MeasurementCommon.Helpers
{
    public static class FaiCollectionHelper
    {
        public static void SetFaiValues(this IFaiCollection faiCollection, IList<FaiItem> faiItems, DateTime measureTime, int cavityNo, string result, string reserved = "")
        {
            if (result == "Empty") return;
            // Assign meta data
            faiCollection.Result = result;
            faiCollection.Cavity = cavityNo;
            faiCollection.MeasureTime = measureTime;
            faiCollection.Reserved = reserved;
            
            // make dictionary for performance reason
            var dict = new Dictionary<string, double>();
            foreach (var faiItem in faiItems)
            {
                dict[faiItem.Name] = faiItem.Value;
            }
            
            // Assign fai values
            var collectionType = faiCollection.GetType();
            var allProps = collectionType.GetProperties();
            var faiProps = allProps.Where(prop => prop.Name.Contains("FAI")).ToList();
            
            if(faiProps.Count!=faiItems.Count) throw new InvalidOperationException("faiProps.Count!=faiItems.Count");


            foreach (var faiProp in faiProps)
            {
                faiProp.SetValue(faiCollection, dict[faiProp.Name]);
            }
        }
        
        
        public static void SetFaiValues(this IFaiCollection faiCollection, Dictionary<string, double> faiDict, DateTime measureTime, int cavityNo, string result, string reserved = "")
        {
            if (result == "Empty") return;
            // Assign meta data
            faiCollection.Result = result;
            faiCollection.Cavity = cavityNo;
            faiCollection.MeasureTime = measureTime;
            faiCollection.Reserved = reserved;

            // Assign fai values
            var collectionType = faiCollection.GetType();
            var allProps = collectionType.GetProperties();
            var faiProps = allProps.Where(prop => prop.Name.Contains("FAI")).ToList();
            
            if(faiProps.Count!=faiDict.Count) throw new InvalidOperationException("faiProps.Count!=faiDict.Count");

            foreach (var faiProp in faiProps)
            {
                faiProp.SetValue(faiCollection, faiDict[faiProp.Name]);
            }
        }
    }
}