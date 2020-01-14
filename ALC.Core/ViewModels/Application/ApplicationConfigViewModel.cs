using System.Xml.Serialization;
using WPFCommon.ViewModels.Base;

namespace ALC.Core.ViewModels.Application
{
    public class ApplicationConfigViewModel : AutoSerializableBase<ApplicationConfigViewModel>
    {
        public static ApplicationConfigViewModel Instance { get; set; }
        
        
        public override string Name => "AppConfigs";

        [XmlAttribute] public string RememberPassword { get; set; } = "123456";
        
        
    }
}