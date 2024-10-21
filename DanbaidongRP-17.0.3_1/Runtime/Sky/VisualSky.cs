using System;

namespace UnityEngine.Rendering.Universal
{
    /// <summary>
    /// Informative enumeration containing SkyUniqeIDs already used by DanbaidongRP.
    /// When users write their own sky type, they can use any ID not present in this enumeration or in their project.
    /// </summary>
    public enum SkyType
    {
        /// <summary>Gradient Sky Unique ID.</summary>
        Gradient = 1,
        /// <summary>HDRI Sky Unique ID.</summary>
        HDRI = 2,
        /// <summary>Procedural ToonSky Unique ID.</summary>
        ProceduralToon = 3,
    }

    /// <summary>
    /// Sky Ambient Mode.
    /// </summary>
    public enum SkyAmbientMode
    {
        /// <summary>DannaidongRP will use the static lighting sky setup in the lighting panel to compute the global ambient probe.</summary>
        Static,
        /// <summary>DannaidongRP will use the current sky used for lighting (either the one setup in the Visual Environment component or the Sky Lighting Override) to compute the global ambient probe.</summary>
        Dynamic,
    }

    [Serializable]
    public sealed class SkyTypeParameter : VolumeParameter<SkyType>
    {
        public SkyTypeParameter(SkyType value, bool overrideState = false) : base(value, overrideState)
        {
        }
    }

    /// <summary>
    /// VisualSky Volume Component.
    /// This component setups the sky used for rendering as well as the way ambient probe should be computed.
    /// </summary>
    [Serializable, VolumeComponentMenu("Sky/VisualSky")]
    [SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
    public sealed class VisualSky : VolumeComponent
    {
        /// <summary>Type of sky that should be used for rendering.</summary>
        [Header("Sky")]
        public SkyTypeParameter skyType = new SkyTypeParameter(SkyType.Gradient, true);
        ///// <summary>Type of clouds that should be used for rendering.</summary>
        //public NoInterpIntParameter cloudType = new NoInterpIntParameter(0);
        ///// <summary>Defines the way the ambient probe should be computed.</summary>
        //public SkyAmbientModeParameter skyAmbientMode = new SkyAmbientModeParameter(SkyAmbientMode.Dynamic);

    }

}