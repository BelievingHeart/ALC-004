﻿﻿using System;

  namespace MeasurementCommon.Fai
{
    public interface IFaiCollection
    {
        /// <summary>
        /// Reserved for other related information
        /// </summary>
        string Reserved { get; set; }
        
         DateTime MeasureTime { get; set; }

         int Cavity { get; set; }
         
         string Result { get; set; }
    }
}