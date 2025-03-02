using Python.Runtime;
using System;
using UnityEngine;
public static class PythonLifeCycle {
  private const string PythonFolder = "python-3.12.7-embed-amd64";
  private const string PythonDll = "python312.dll";
  private const string PythonZip = "python312.zip";
  private const string PythonProject = "Scripts";
  private const string CUDAFolder = "CUDA";
  private const string CUDAVersion = "v12.4";
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] private static void PythonInitialize() {
    Application.quitting += PythonShutdown;
    Initialize(PythonProject);
  }
  private static void PythonShutdown() {
    Application.quitting -= PythonShutdown;
    Shutdown();
  }
  public static void Initialize(string appendPythonPath = "") {
    var pythonHome = $"{Application.streamingAssetsPath}/{PythonFolder}";
    var appendPath = string.IsNullOrWhiteSpace(appendPythonPath) ? string.Empty : $"{Application.streamingAssetsPath}/{appendPythonPath}";
    var pythonPath = string.Join(
      ";",
      $"{appendPath}",
      $"{pythonHome}/Lib/site-packages",
      $"{pythonHome}/{PythonZip}",
      $"{pythonHome}"
    );
    var scripts = $"{pythonHome}/Scripts";
    var cudaHome = $"{Application.streamingAssetsPath}/{CUDAFolder}/{CUDAVersion}";
    var cudaPath = string.Join(
      ";",
      $"{cudaHome}/bin",
      $"{cudaHome}/libnvvp",
      $"{cudaHome}/extras/CUPTI/lib64",
      $"{cudaHome}/include"
    );
    var path = Environment.GetEnvironmentVariable("PATH")?.TrimEnd(';');
    path = string.IsNullOrEmpty(path) ? $"{pythonHome};{scripts};{cudaPath};" : $"{pythonHome};{scripts};{cudaPath};{path};";
    Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);
    Environment.SetEnvironmentVariable("DYLD_LIBRARY_PATH", $"{pythonHome}/Lib", EnvironmentVariableTarget.Process);
    Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", $"{pythonHome}/{PythonDll}", EnvironmentVariableTarget.Process);
    #if UNITY_EDITOR
      Environment.SetEnvironmentVariable("PYTHONDONTWRITEBYTECODE", "1", EnvironmentVariableTarget.Process);
    #endif
    PythonEngine.PythonHome = pythonHome;
    PythonEngine.PythonPath = pythonPath;
    PythonEngine.Initialize();
  }
  public static void Shutdown() {
    PythonEngine.Shutdown();
  }
}
