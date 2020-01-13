﻿using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace WPFCommon.ViewModels.Base
{
    public class AutoSerializableBase<T> : ViewModelBase
    {
        /// <summary>
        /// Fai name
        /// </summary>
        [XmlIgnore]
        public virtual string Name { get; set; }

        /// <summary>
        /// Whether the object should be auto-serialize when changed
        /// </summary>
        [XmlIgnore]
        public bool ShouldAutoSerialize { get; set; }

        public AutoSerializableBase()
        {
            PropertyChanged += SerializeConfigurations;
        }

        public void SerializeConfigurations(object sender, PropertyChangedEventArgs e)
        {
            if (!ShouldAutoSerialize) return;
            if (string.IsNullOrEmpty(SerializationDirectory)) return;
            if (string.IsNullOrEmpty(Name)) return;

            using (var fs = new FileStream(Path.Combine(SerializationDirectory, Name + ".xml")
                , FileMode.Create))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(fs, this);
            }
        }

        [XmlIgnore] public string SerializationDirectory { get; set; }
    }
}