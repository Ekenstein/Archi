﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Archi.Core {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ArchiResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ArchiResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Archi.Core.ArchiResources", typeof(ArchiResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to create the file &apos;{0}&apos;..
        /// </summary>
        internal static string FailedToCreateFile {
            get {
                return ResourceManager.GetString("FailedToCreateFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to remove the file &apos;{0}&apos;..
        /// </summary>
        internal static string FailedToRemoveFile {
            get {
                return ResourceManager.GetString("FailedToRemoveFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The store does not implement IArchiveFileStore&lt;TArchive&gt;..
        /// </summary>
        internal static string StoreNotIArchiveFileStore {
            get {
                return ResourceManager.GetString("StoreNotIArchiveFileStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The store does not implement IArchiveTagStore&lt;TArchive&gt;..
        /// </summary>
        internal static string StoreNotIArchiveTagStore {
            get {
                return ResourceManager.GetString("StoreNotIArchiveTagStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The store does not implement IQueryableArchiveStore&lt;TArchive&gt;..
        /// </summary>
        internal static string StoreNotIQueryableArchiveStore {
            get {
                return ResourceManager.GetString("StoreNotIQueryableArchiveStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The tag &apos;{0}&apos; is already associated with the archive..
        /// </summary>
        internal static string TagAlreadyExist {
            get {
                return ResourceManager.GetString("TagAlreadyExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The tag &apos;{0}&apos; is not associated with the archive..
        /// </summary>
        internal static string TagDoesNotExist {
            get {
                return ResourceManager.GetString("TagDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The tag &apos;{0}&apos; contains invalid characters..
        /// </summary>
        internal static string TagInvalid {
            get {
                return ResourceManager.GetString("TagInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An archive tag must not be null or white space..
        /// </summary>
        internal static string TagMustNotBeNullOrEmpty {
            get {
                return ResourceManager.GetString("TagMustNotBeNullOrEmpty", resourceCulture);
            }
        }
    }
}
