﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using Microsoft.JSON.Core.Parser;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace JSON_Intellisense.NPM
{
    [Export(typeof(IJSONSmartTagProvider))]
    [Name("Npm Install Package")]
    [Order(After = "NPM Update Package")]
    class InstallPackageProvider : JSONSmartTagProviderBase
    {
        public override string SupportedFileName
        {
            get { return Constants.FileName; }
        }

        public override IEnumerable<ISmartTagAction> GetSmartTagActions(JSONMember item, ITextBuffer buffer)
        {
            string directory = Path.GetDirectoryName(buffer.GetFileName());

            if (item.Value != null && item.Value.Text.Trim('"').Length > 0)
                yield return new InstallPackageAction(item, directory);
        }
    }

    internal class InstallPackageAction : JSONSmartTagActionBase
    {
        private JSONMember _item;
        private string _directory;

        public InstallPackageAction(JSONMember item, string directory)
        {
            _item = item;
            _directory = directory;
            Icon = Constants.Icon;
        }

        public override string DisplayText
        {
            get { return "Install package"; }
        }

        public override void Invoke()
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo("cmd", "/k npm install " + _item.UnquotedNameText)
                {
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                    WorkingDirectory = _directory
                }
            };

            p.Start();
            p.Dispose();
        }
    }
}
