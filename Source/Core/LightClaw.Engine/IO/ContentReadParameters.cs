using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.IO
{
    /// <summary>
    /// Parameters for an <see cref="IContentReader"/>.
    /// </summary>
    public sealed class ContentReadParameters : ICloneable, IEquatable<ContentReadParameters>
    {
        /// <summary>
        /// The <see cref="IContentManager"/> that triggered the loading process.
        /// </summary>
        public IContentManager ContentManager { get; private set; }

        /// <summary>
        /// The resource string of the asset to be loaded.
        /// </summary>
        public ResourceString ResourceString { get; private set; }

        /// <summary>
        /// A <see cref="Stream"/> of the asset's data.
        /// </summary>
        public Type AssetType { get; private set; }

        /// <summary>
        /// The <see cref="Type"/> of asset to read.
        /// </summary>
        public Stream AssetStream { get; private set; }

        /// <summary>
        /// A parameter the client specifies when requesting an asset. This may be null.
        /// </summary>
        public object Parameter { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="ContentReadParameters"/>-instance.
        /// </summary>
        /// <param name="contentManager">The <see cref="IContentManager"/> that triggered the loading process.</param>
        /// <param name="resourceString">The resource string of the asset to be loaded.</param>
        /// <param name="assetType">A <see cref="Stream"/> of the asset's data.</param>
        /// <param name="assetStream">The <see cref="Type"/> of asset to read.</param>
        /// <param name="parameter">A parameter the client specifies when requesting an asset. This may be null.</param>
        public ContentReadParameters(IContentManager contentManager, ResourceString resourceString, Type assetType, Stream assetStream, object parameter)
        {
            Contract.Requires<ArgumentNullException>(contentManager != null);
            Contract.Requires<ArgumentNullException>(assetType != null);
            Contract.Requires<ArgumentNullException>(assetStream != null);

            this.ContentManager = contentManager;
            this.ResourceString = resourceString;
            this.AssetType = assetType;
            this.AssetStream = assetStream;
            this.Parameter = parameter;
        }

        /// <summary>
        /// Creates a clone of the object.
        /// </summary>
        /// <returns>The cloning result.</returns>
        public object Clone()
        {
            return new ContentReadParameters(this.ContentManager, this.ResourceString, this.AssetType, this.AssetStream, this.Parameter);
        }

        /// <summary>
        /// Tests for equality with the specified <see cref="Object"/>.
        /// </summary>
        /// <param name="obj">The object to test against.</param>
        /// <returns><c>true</c> if the <see cref="ContentReadParameters"/> is equal to the specified object, otherwise <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            ContentReadParameters parameters = obj as ContentReadParameters;
            return (parameters != null) ? this.Equals(parameters) : false;
        }

        /// <summary>
        /// Tests for equality with the <paramref name="other"/> specified <see cref="ContentReadParameters"/>.
        /// </summary>
        /// <param name="other"><see cref="ContentReadParameters"/> to test against.</param>
        /// <returns><c>true</c> if the <see cref="ContentReadParameters"/> are equal, otherwise <c>false</c>.</returns>
        public bool Equals(ContentReadParameters other)
        {
            if (ReferenceEquals(other, null))
                return false;
            if (ReferenceEquals(other, this))
                return true;

            return (this.ContentManager == other.ContentManager) && (this.ResourceString == other.ResourceString) &&
                   (this.AssetType == other.AssetType) && (this.AssetStream == other.AssetStream) &&
                   (this.Parameter == other.Parameter);
        }

        /// <summary>
        /// Gets the objects hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.ContentManager, this.ResourceString, this.AssetType, this.AssetStream, this.Parameter);
        }

        /// <summary>
        /// Checks whether two <see cref="ContentReadParameters"/> are equal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="ContentReadParameters"/> are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(ContentReadParameters left, ContentReadParameters right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether two <see cref="ContentReadParameters"/> are inequal.
        /// </summary>
        /// <param name="left">The first operand.</param>
        /// <param name="right">The second operand.</param>
        /// <returns><c>true</c> if the <see cref="ContentReadParameters"/> are inequal, otherwise <c>false</c>.</returns>
        public static bool operator !=(ContentReadParameters left, ContentReadParameters right)
        {
            return !(left == right);
        }
    }
}
