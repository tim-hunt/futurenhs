﻿namespace MvcForum.Web.ViewModels.Admin
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.Models.Entities;

    #region Members

    public class BatchDeleteMembersViewModel
    {
        [Required]
        [Range(0, int.MaxValue)]
        [DisplayName("Registered in the last specified amount of days")]
        public int AmoutOfDaysSinceRegistered { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [DisplayName("And has posted less than or equal to this amount of times")]
        public int AmoutOfPosts { get; set; }

        public int AmountDeleted { get; set; }
    }

    #endregion

    #region Topics

    public class BatchMoveTopicsViewModel
    {
        public List<Group> Groups { get; set; }

        [Required]
        [DisplayName("Move all Topics in this Group")]
        public Guid? FromGroup { get; set; }

        [Required]
        [DisplayName("To this new Group")]
        public Guid? ToGroup { get; set; }
    }

    #endregion

    #region Private Messages

    public class BatchDeletePrivateMessagesViewModel
    {
        [Required]
        [DisplayName("How many Days?")]
        public int Days { get; set; }
    }

    #endregion
}