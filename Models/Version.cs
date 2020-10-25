using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;

namespace ValueCards.Models
{
  public class AssemblyVersion
  {
    public string Version { get; set; }
    public string Build { get; set; }
    [Display(Name = "Release date")]
    public DateTime ReleaseDate { get; set; }


    public AssemblyVersion()
    {
      Assembly asm = Assembly.GetExecutingAssembly();
      AssemblyName asn = new AssemblyName(asm.FullName);
      Build = string.Format("{0}-{1}", asn.Version.Build, asn.Version.MinorRevision);

      var version = asm.GetCustomAttribute<AssemblyFileVersionAttribute>();
      if (version != null)
      {
        Version = version.Version;
      }
      else
      {
        Version = asn.Version.ToString();
      }

      ReleaseDate = File.GetLastWriteTime(asm.ManifestModule.FullyQualifiedName).ToUniversalTime();
    }
  }
}
