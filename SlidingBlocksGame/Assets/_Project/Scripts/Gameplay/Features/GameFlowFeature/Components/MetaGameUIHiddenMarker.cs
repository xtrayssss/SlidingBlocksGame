using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

<<<<<<<< HEAD:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/GameFlowFeature/Components/MetaGameUIHiddenMarker.cs
namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Components
========
namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components
>>>>>>>> recovery-branch:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/VisualFeature/UIFeature/Components/MetaGameUIHiddenMarker.cs
{
    [Serializable]
    public struct MetaGameUIHiddenMarker : IEcsTagComponent
    {

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.GameFlowFeature.Components", sourceClassName: "MetaGameUIHiddenMarker/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : TagComponentTemplate<MetaGameUIHiddenMarker>
        {
        }
    }
}