using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kartel.Trade.Web.Areas.ControlPanel.Models
{
    /// <summary>
    /// ������� ������ ���� ������ ExtJS 
    /// </summary>
    public class ExtJSTreeNodeModel
    {
        /// <summary>
        /// ������������� ����
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// ����� � ����
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// ������������ ��������� � ����
        /// </summary>
        [JsonProperty("qtip")]
        public string Tooltip { get; set; }

        /// <summary>
        /// ����� ������
        /// </summary>
        [JsonProperty("iconCls")]
        public string IconClass { get; set; }

        /// <summary>
        /// ������ �� ���� ������
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// URL ��� �������� ��� ����� �� ����
        /// </summary>
        [JsonProperty("href")]
        public string Href { get; set; }

        /// <summary>
        /// �������� �� ���� �� ������� ��������
        /// </summary>
        [JsonProperty("leaf")]
        public bool Leaf { get; set; }

        /// <summary>
        /// ������� �� ���� �� ���������
        /// </summary>
        [JsonProperty("expanded")]
        public bool Expanded { get; set; }

        /// <summary>
        /// �������� ����
        /// </summary>
        [JsonProperty("children")]
        public List<ExtJSTreeNodeModel> Childrens { get; set; }

        /// <summary>
        /// ��������� �� ������� + ��������� ��������
        /// </summary>
        [JsonProperty("checked")]
        public bool? Checked { get; set; }
    }
}