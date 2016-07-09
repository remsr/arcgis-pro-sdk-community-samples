﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using LayerPopups.Helpers;

namespace LayerPopups {
    /// <summary>
    /// Create a simple layer popup that has a title and list of field values
    /// </summary>
    /// <remarks>Requires the States dataset from the AdminData.gdb downloadable from:
    /// <a href="https://github.com/Esri/arcgis-pro-sdk-community-samples/releases"/></remarks>
    internal class SimplePopup : Button {
        protected override void OnClick() {
            string layerName = "States";
            var usStatesLayer = MapView.Active.Map.GetLayersAsFlattenedList()
                .FirstOrDefault((lyr) => lyr.Name == layerName) as FeatureLayer;
            if (usStatesLayer == null) {
                MessageBox.Show(
                    "Please add the 'States' layer to the TOC from the Pro community samples AdminData.gdb geodatabase.", 
                    "Cannot find US States");
                return;
            }
            //we do not need to await it
            CreateSimplePopupAsync(usStatesLayer);

            MessageBox.Show(
                "Popup for U.S. States applied. Use the explore tool to identify a state and examine its popup",
                "U.S. States Simple Popup");

            FrameworkApplication.SetCurrentToolAsync("esri_mapping_exploreTool");
        }

        private Task CreateSimplePopupAsync(FeatureLayer fl) {
            return QueuedTask.Run(() => {
                var def = fl.GetDefinition() as CIMFeatureLayer;
                string popupText = string.Format("{0} ({1}), population {2}",
                    PopupDefinition.FormatFieldName("STATE_NAME"),
                    PopupDefinition.FormatFieldName("STATE_ABBR"),
                    PopupDefinition.FormatFieldName("TOTPOP2010"));
                //Create a popup definition with text and table
                //Just add all the table fields by default
                PopupDefinition popup = new PopupDefinition() {
                    Title = PopupDefinition.FormatTitle(
                        string.Format("{0} (Simple Popup)", PopupDefinition.FormatFieldName(def.FeatureTable.DisplayField))),
                    TextMediaInfo = new TextMediaInfo() {
                        Text = PopupDefinition.FormatText(popupText)
                    },
                    TableMediaInfo = new TableMediaInfo(fl.GetFeatureClass().GetDefinition().GetFields())   
                };

                fl.SetPopupInfo(popup.CreatePopupInfo());
            });
        }
    }
}