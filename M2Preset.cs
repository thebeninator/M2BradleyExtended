using Presets;

namespace M2BradleyExtended
{
    internal class M2Preset : PresetTemplate
    {
        public bool AddonArmour { get; set; }
        public bool M919 { get; set; }
        public string TOWMissile { get; set; } = "Default";
        public bool LRF { get; set; }
        public bool IBAS { get; set; }
        public bool EnhancedM242 { get; set; }
        public bool XM913 { get; set; }
        public bool QuickRefillBins { get; set; }
    }
}
