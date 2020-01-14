﻿using System;
using System.Xml.Serialization;
using WPFCommon.ViewModels.Base;

namespace Core.ViewModels.Fai
{
    public sealed partial class FaiItem : AutoSerializableBase<FaiItem>
    {
        #region attribute

        /// <summary>
        /// Max boundary of the fai item
        /// </summary>
        [XmlAttribute]public double MaxBoundary { get; set; }

        /// <summary>
        /// Min boundary of the fai item
        /// </summary>
        [XmlAttribute]public double MinBoundary { get; set; }
        
        [XmlAttribute] public double Weight { get; set; } = 1;

        /// <summary>
        /// Bias 
        /// </summary>
        [XmlAttribute] public double Bias { get; set; }

        #endregion

        #region ignore

        /// <summary>
        /// Measured value
        /// </summary>
        [XmlIgnore]
        public double ValueUnbiased { get; set; }

        /// <summary>
        /// Measured value plus bias
        /// </summary>
        [XmlIgnore]
        public double Value => Math.Abs(ValueUnbiased * Weight + Bias);




        /// <summary>
        /// Measure result
        /// </summary>
        [XmlIgnore]
        public bool Rejected
        {
            get { return Value < MinBoundary || Value > MaxBoundary; }
        }

        
        [XmlIgnore]
        public bool Passed
        {
            get { return !Rejected; }
        }


        #endregion
        
        public FaiItem Clone()
        {
            return new FaiItem()
            {
                Name = Name,
                MinBoundary = MinBoundary,
                MaxBoundary = MaxBoundary,
                ValueUnbiased = ValueUnbiased,
                Weight = Weight,
                Bias = Bias,
            };
        }

    }
}