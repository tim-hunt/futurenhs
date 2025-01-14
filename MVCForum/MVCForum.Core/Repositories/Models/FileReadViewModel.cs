﻿namespace MvcForum.Core.Repositories.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Defines the file entity to store file meta-data.
    /// </summary>
    public class FileReadViewModel
    {
        /// <summary>
        /// Gets or sets the file id.
        /// </summary>
        [Column(name: "Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the file title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the parent folder.
        /// </summary>
        public Guid ParentFolder { get; set; }

        /// <summary>
        /// Gets or sets the file description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file size.
        /// </summary>
        public string FileSize { get; set; }

        /// <summary>
        /// Gets or sets the file extension.
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Gets or sets the file url.
        /// </summary>
        public string BlobName { get; set; }

        /// <summary>
        /// Gets or sets the file created by.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the file modified by.
        /// </summary>
        public Guid? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the file created date.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the file modified date.
        /// </summary>
        public DateTime? ModifiedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the status. Relates to UploadStatus Id.
        /// </summary>
        public int Status { get; set; }

        public string UserName { get; set; }
        public string UserSlug { get; set; }

        /// <summary>
        /// Will return name of user that created the file if modifed by is null.
        /// </summary>
        public string ModifiedUserName { get; set; }

        /// <summary>
        /// Will return the slug of user that created the file if modifed by is null.
        /// </summary>
        public string ModifiedUserSlug { get; set; }

        /// <summary>
        /// Will return the date the file was created if modifed at is null.
        /// </summary>
        public DateTime LastModifiedAtUtc { get; set; }
    }
}
