using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SubExplore.Models.Media
{
    /// <summary>
    /// Interface pour représenter un fichier média dans l'application mobile
    /// Remplace l'usage de IFormFile
    /// </summary>
    public interface IMediaFile
    {
        /// <summary>
        /// Nom du fichier
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Type de contenu (MIME)
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Taille du fichier en octets
        /// </summary>
        long Length { get; }

        /// <summary>
        /// Chemin du fichier
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Type MIME du fichier
        /// </summary>
        string MimeType { get; }

        /// <summary>
        /// Taille du fichier
        /// </summary>
        long Size { get; }

        /// <summary>
        /// Indique si c'est le fichier principal
        /// </summary>
        bool IsPrimary { get; }

        /// <summary>
        /// Ouvre un stream pour lire le contenu du fichier
        /// </summary>
        Stream OpenReadStream();

        /// <summary>
        /// Copie le contenu du fichier vers un stream
        /// </summary>
        Task CopyToAsync(Stream target, CancellationToken cancellationToken = default);
    }
}