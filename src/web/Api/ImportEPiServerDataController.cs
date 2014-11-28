/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Specialized;
using System.IO;
using System.Web.Mvc;
using Castle.Core.Internal;
using EPiServer.Core;
using EPiServer.Core.Transfer;
using EPiServer.Enterprise;
using EPiServer.Enterprise.Transfer;
using log4net;

namespace OxxCommerceStarterKit.Web.Api
{
    public class ImportEPiServerDataController : BaseApiController
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(ImportEPiServerDataController));


        public string Get()
        {
            using (FileStream fileStream = new FileStream("e:\\temp\\SampleAssets.episerverdata", FileMode.Open))
            {
                return "Size of file: " + fileStream.Length;
            }
        }

        [HttpPost]
        public ImportResult Import(bool isTest)
        {
            return ImportFileThread(isTest);
        }

        [HttpPost]
        public ImportResult Abort()
        {
            if (ExportImportBase.CurrentContext != null)
            {
                ITransferContext dataImporter = ExportImportBase.CurrentContext;
                ImportResult result = new ImportResult();
                result.Errors = dataImporter.Log.Errors;
                result.Warnings = dataImporter.Log.Warnings;
                dataImporter.Abort();
                return result;
            }
            return null;
        }


        [HttpGet]
        public ImportResult GetStatus()
        {
            if (ExportImportBase.CurrentContext != null)
            {
                ITransferContext dataImporter = ExportImportBase.CurrentContext;
                ImportResult result = new ImportResult();
                result.Errors = dataImporter.Log.Errors;
                result.Warnings = dataImporter.Log.Warnings;
                return result;
            }
            return null;
        }


        private ImportResult ImportFileThread(bool isTest)
        {
            DataImporter.ContentImporting += OnContentImporting;
            DataImporter.FileImporting += OnFileImporting;

            DataImporter dataImporter = new DataImporter();
            dataImporter.Stream = new FileStream("e:\\temp\\SampleAssets.episerverdata", FileMode.Open);
            dataImporter.IsTest = false;
            dataImporter.DestinationRoot = ContentReference.GlobalBlockFolder; // .StartPage;
            dataImporter.KeepIdentity = true;
            // dataImporter.ContinueOnError = true;
            dataImporter.AutoCloseStream = true;
            dataImporter.IsTest = isTest;
            dataImporter.TransferType = TypeOfTransfer.Importing;

            dataImporter.Import();
            ImportResult result = new ImportResult();
            result.Errors = dataImporter.Log.Errors;
            result.Warnings = dataImporter.Log.Warnings;

            DataImporter.ContentImporting -= OnContentImporting;
            DataImporter.FileImporting -= OnFileImporting;

            return result;
        }

        private void OnContentImporting(DataImporter importing, ContentImportingEventArgs args)
        {

            RawProperty property = args.TransferContentData.RawContentData.Property.Find(p => p.Name.Equals("PageName"));
            if (property == null)
                property = args.TransferContentData.RawContentData.Property[0];

            _log.DebugFormat("Content Import: {0} = {1}", property.Name, property.Value);

        }

        private void OnFileImporting(DataImporter importing, FileImportingEventArgs args)
        {
            _log.Debug("File import: " + args.PermanentLinkVirtualPath);
        }
    }

    public class ImportResult
    {
        public StringCollection Errors { get; set; }
        public StringCollection Warnings { get; set; }


    }
}
