/*
 *	Copyright (c) 2020, AndrewMJordan
 *	All rights reserved.
 *	
 *	This source code is licensed under the BSD-style license found in the
 *	LICENSE file in the root directory of this source tree
 */

using System;
using System.IO;
using UnityEditor;

namespace Andtech.BuildUtility {

	public class BuildHelper {
		public readonly string[] Args;

		public BuildHelper(string[] args) {
			Args = args;
		}

		public bool TryGetArgument(string name, out string value) {
			for (int i = 0; i < Args.Length; i++) {
				var arg = Args[i];
				var match = arg.Equals("-" + name);
				if (match) {
					value = Args[i + 1];
					return true;
				}
			}

			value = null;
			return false;
		}

		public BuildTarget GetBuildTarget() {
			BuildTarget buildTarget;

			var hasBuildTarget = TryGetArgument("buildTarget", out string buildTargetName);
			if (hasBuildTarget) {
				if (Enum.TryParse(buildTargetName, out buildTarget)) {
					Console.WriteLine(":: Received custom build target " + buildTargetName);
				}
				else {
					buildTarget = EditorUserBuildSettings.activeBuildTarget;
					Console.WriteLine($":: {nameof(buildTargetName)} \"{buildTargetName}\" not defined on enum {nameof(BuildTarget)}, using {buildTarget} enum to build");
				}
			}
			else {
				buildTarget = EditorUserBuildSettings.activeBuildTarget;
			}

			return buildTarget;
		}

		public string GetOutputPath() => GetOutputPath(GetBuildTarget());

		public string GetOutputPath(BuildTarget buildTarget) {
			var extension = GetExtension(buildTarget);

			var defaultDirectory = "Builds";
			var defaultFilename = string.Concat(PlayerSettings.productName, extension);

			string outputDirectory = defaultDirectory;
			string outputFilename = defaultFilename;
			if (TryGetArgument("output", out var output)) {
				if (Path.HasExtension(output)) {
					outputDirectory = Path.GetDirectoryName(output);
					outputFilename = Path.GetFileName(output);
				}
				else {
					outputDirectory = output;
					outputFilename = defaultFilename;
				}
			}

			if (TryGetArgument("name", out string name)) {
				outputFilename = Path.HasExtension(name) ? name : string.Concat(name, extension);
			}

			return Path.Combine(outputDirectory, outputFilename);
		}

		public string GetExtension(BuildTarget buildTarget) {
			switch (buildTarget) {
				case BuildTarget.StandaloneOSX:
					return ".app";
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
					return ".exe";
				case BuildTarget.iOS:
					return string.Empty;
				case BuildTarget.Android:
					return ".apk";
				case BuildTarget.StandaloneLinux:
					break;
				case BuildTarget.WebGL:
					break;
				case BuildTarget.WSAPlayer:
					break;
				case BuildTarget.StandaloneLinux64:
					break;
				case BuildTarget.StandaloneLinuxUniversal:
					break;
				case BuildTarget.Tizen:
					break;
				case BuildTarget.PSP2:
					break;
				case BuildTarget.PS4:
					break;
				case BuildTarget.PSM:
					break;
				case BuildTarget.XboxOne:
					break;
				case BuildTarget.N3DS:
					break;
				case BuildTarget.WiiU:
					break;
				case BuildTarget.tvOS:
					break;
				case BuildTarget.Switch:
					break;
				case BuildTarget.NoTarget:
					break;
				default:
					break;
			}

			return ".unknown";
		}
	}
}
