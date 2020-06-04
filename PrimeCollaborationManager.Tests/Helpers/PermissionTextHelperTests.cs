using Xunit;
using PrimeCollaborationManager.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrimeCollaborationManager.Helpers.Tests
{
    public class PermissionTextHelperTests
    {
        [Theory]
        [InlineData("","")]
        [InlineData("asdf", "asdf")]
        [InlineData("SaveCopy", "Save As")]
        [InlineData("PrintCopy", "Print")]
        [InlineData("Markup", "Markup")]
        [InlineData("AddDocuments", "Add Documents")]
        [InlineData("MarkupAlert", "Markup Alert")]
        [InlineData("UndoCheckouts", "Revoke Check Out")]
        [InlineData("CreateSessions", "Send PDFs to Studio Sessions")]
        [InlineData("ShareItems", "Share File Links")]
        [InlineData("Invite", "Send Invitations")]
        [InlineData("ManageParticipants", "Manage User Access")]
        [InlineData("ManagePermissions", "Manage Permissions")]
        [InlineData("FullControl", "Full Control")]
        public void GetPermissionDisplayNameTest(string input, string output)
        {
            Assert.Equal(output, PermissionTextHelper.GetPermissionDisplayName(input));
        }

        [Fact]
        public void GetPermissionDisplayName_NullTest()
        {
            Assert.Throws<ArgumentNullException>(() => PermissionTextHelper.GetPermissionDisplayName(null));
        }
    }
}