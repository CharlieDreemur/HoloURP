using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class About
{
    public static void AboutGUI() {
        GUILayout.BeginHorizontal();
        GUILayout.Label("This string was corrupted and causing compile errors so it was replaced to this.");
        GUILayout.EndHorizontal();
    }
}
