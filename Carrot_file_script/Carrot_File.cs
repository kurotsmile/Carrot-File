using SFB;
using SimpleFileBrowser;
using System.Collections.Generic;
using UnityEngine;

namespace Carrot
{
    public enum Carrot_File_Type {SimpleFileBrowser,StandaloneFileBrowser}
    public class Carrot_File_Query
    {
        public List<string> s_title_file_data;
        public List<string[]> list_extension_file;
        public string s_DefaultFilter_extension_file = "";

        public Carrot_File_Query()
        {
            this.s_title_file_data= new List<string>();
            this.list_extension_file=new List<string[]>();
        }

        public void SetDefaultFilter(string s_extension_file)
        {
            this.s_DefaultFilter_extension_file = s_extension_file;
        }
        public void Add_filter(string title_type_file_data, params string[] extension_file)
        {
            s_title_file_data.Add(title_type_file_data);
            list_extension_file.Add(extension_file);
        }
    }

    public class Carrot_File : MonoBehaviour
    {
        [Header("Main")]
        public Carrot_File_Type type = Carrot_File_Type.SimpleFileBrowser;

        private FileBrowser.Filter[] SimpleFileBrowser_filter;
        private ExtensionFilter[] StandaloneFileBrowser_filter;

        private void Handle_filter(Carrot_File_Query query)
        {
            if (this.type == Carrot_File_Type.SimpleFileBrowser)
            {
                this.SimpleFileBrowser_filter = new FileBrowser.Filter[query.list_extension_file.Count];
                for (int i = 0; i < query.list_extension_file.Count; i++)
                {

                    string[] extension_file = new string[query.list_extension_file[i].Length];
                    string[] extension_file_handle = new string[extension_file.Length];

                    for (int y = 0; y < extension_file.Length; y++)
                    {
                        extension_file_handle[y] = "." + query.list_extension_file[i][y];
                    }
                    this.SimpleFileBrowser_filter[i] = new FileBrowser.Filter(query.s_title_file_data[i], extension_file_handle);
                }
                FileBrowser.SetFilters(true, this.SimpleFileBrowser_filter);
                if (query.s_DefaultFilter_extension_file != "") FileBrowser.SetDefaultFilter("." + query.s_DefaultFilter_extension_file);
            }
            else
            {
                this.StandaloneFileBrowser_filter = new ExtensionFilter[query.list_extension_file.Count + 1];

                for (int i = 0; i < this.StandaloneFileBrowser_filter.Length - 1; i++)
                {
                    string[] extension_file = query.list_extension_file[i];
                    this.StandaloneFileBrowser_filter[i] = new ExtensionFilter(query.s_title_file_data[i], extension_file);
                }

                this.StandaloneFileBrowser_filter[this.StandaloneFileBrowser_filter.Length - 1] = new ExtensionFilter("All Files", "*");
            }
        }

        public void Open_file(Carrot_File_Query query,FileBrowser.OnSuccess Act_done,FileBrowser.OnCancel Act_cancel)
        {
            this.Handle_filter(query);
            if (this.type == Carrot_File_Type.SimpleFileBrowser) { 
                FileBrowser.ShowLoadDialog(Act_done, Act_cancel, FileBrowser.PickMode.Files, false);
            }
            else
            {
                StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", this.StandaloneFileBrowser_filter, false, (string[] paths) => {
                    if (paths.Length > 0)
                        Act_done(paths);
                    else
                        Act_cancel();
                });
            }
        }

        public void Save_file(Carrot_File_Query query,FileBrowser.OnSuccess Act_done, FileBrowser.OnCancel Act_cancel)
        {
            this.Handle_filter(query);
            if (this.type == Carrot_File_Type.SimpleFileBrowser)
            {
                FileBrowser.ShowSaveDialog(Act_done, Act_cancel, FileBrowser.PickMode.Files, false);
            }
            else
            {
                StandaloneFileBrowser.SaveFilePanelAsync("Save File", "","Myfile", this.StandaloneFileBrowser_filter,(string path) => {
                    if (path != "")
                    {
                        string[] paths = new string[1];
                        paths[0] = path;
                        Act_done(paths);
                    }
                    else
                    {
                        Act_cancel();
                    } 
                });
            }
        }
    }
}
