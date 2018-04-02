using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;

namespace XHFrame
{
    [CustomEditor(typeof(ReferenceManage))]
    public class ReferenceCustomEditor : Editor
    {
        private ReferenceManage rm;
        private void OnEnable()
        {
            rm = (ReferenceManage)target;
        }

        public override void OnInspectorGUI()
        {

            // 空格换行*2
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("热更新脚本名");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            rm.ButtJoinHorUpdateScript = EditorGUILayout.TextField(rm.ButtJoinHorUpdateScript);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("生成的脚本路径");
            rm.path = EditorGUILayout.TextField(rm.path);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();


            if (GUILayout.Button("添加", GUILayout.Height(25)))
            {
                rm.objectList.Add(new ReferenceData());
            }

            if (GUILayout.Button("删除", GUILayout.Height(25)))
            {
                for (int i = rm.objectList.Count - 1; i >= 0; i--)
                {
                    var item = rm.objectList[i];
                    if (item.deleteField)
                        rm.objectList.Remove(item);
                }
            }

            if (GUILayout.Button("删除空引用", GUILayout.Height(25)))
            {
                for (int i = rm.objectList.Count - 1; i >= 0; i--)
                {
                    var item = rm.objectList[i];
                    if (item.Object == null)
                        rm.objectList.Remove(item);
                }
            }

            if (GUILayout.Button("删除全部引用", GUILayout.Height(25)))
            {
                for (int i = rm.objectList.Count - 1; i >= 0; i--)
                {
                    rm.objectList.Clear();
                }
            }



            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("选择", GUILayout.Width(50));
            EditorGUILayout.LabelField("引用变量名");
            EditorGUILayout.LabelField("引用");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            for (int i = 0; i < rm.objectList.Count; i++)
            {
                var item = rm.objectList[i];

                EditorGUILayout.BeginHorizontal();
                item.deleteField = EditorGUILayout.Toggle(item.deleteField, GUILayout.Width(20));
                if (item.Object != null)
                    item.name = EditorGUILayout.TextField(item.Object.name.Replace(" ", "").Replace(item.Object.name[0].ToString(), item.Object.name[0].ToString().ToLower()).Replace('(', '_').Replace(')', '_'));

                else
                    item.name = EditorGUILayout.TextField(item.name);
                item.Object = EditorGUILayout.ObjectField(item.Object, typeof(UnityEngine.Object), true);
                if (GUILayout.Button("删除", GUILayout.Width(40), GUILayout.Height(17)))
                {
                    rm.objectList.Remove(item);
                }
                EditorGUILayout.EndHorizontal();
            }


            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("生成热更新引用脚本", GUILayout.Height(25)))
            {
                List<string> nameList = new List<string>();
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.Append("using HotUpdateMessage; \n");
                strBuilder.Append("using System; \n");
                strBuilder.Append("using System.Collections; \n");
                strBuilder.Append("using System.Collections.Generic; \n");
                strBuilder.Append("using UnityEngine; \n \n");
                strBuilder.Append("namespace " + Defines.HorUpdateScriptnNamespace + "\n");
                strBuilder.Append(" { \n ");
                strBuilder.Append("    public class " + rm.ButtJoinHorUpdateScript + "\n");
                strBuilder.Append("     { \n");
                int i = 0;
                foreach (var item in rm.objectList)
                {
                    i++;
                    if (nameList.Contains(item.name))
                    {
                        Debug.LogError("引用管理器中<<color=blue>第" + i + "行</color>> 的引用出现重复!!或者名字出现了重复,请检查并修改");
                        return;
                    }
                    nameList.Add(item.name);
                    strBuilder.Append("        public " + item.Object.GetType() + " " + item.name + ";" + "\n");
                }
                nameList.Clear();
                strBuilder.Append("\n \n    } \n");
                strBuilder.Append("}");
                string script = strBuilder.ToString();
                string filePath = null;
                if (string.IsNullOrEmpty(rm.ButtJoinHorUpdateScript))
                {
                    Debug.LogError("类名不能为空! 请输入类名!!!");
                    return;
                }
                if (string.IsNullOrEmpty(rm.path))
                {
                    filePath = Directory.GetCurrentDirectory() + @"\HorUpdateDLL\Handler" + rm.ButtJoinHorUpdateScript;
                }
                else
                {
                    filePath = rm.path + @"\" + rm.ButtJoinHorUpdateScript;
                }

                try
                {
                    if (!Directory.Exists(rm.path))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    if (File.Exists(filePath + ".cs"))
                    {
                        File.Delete(filePath + ".cs");
                    }
                    using (FileStream fs = new FileStream(filePath + ".cs", FileMode.Append, FileAccess.Write))
                    {
                        Debug.Log(filePath);
                        fs.Lock(0, fs.Length);
                        StreamWriter sw = new StreamWriter(fs);
                        sw.WriteLine(script);
                        fs.Unlock(0, fs.Length);
                        sw.Flush();
                    }
                    Debug.LogWarning("热更新脚本创建成功,请移至脚本目录 \n保存的目录是:" + rm.path);

                }
                catch (Exception)
                {
                    Debug.LogError("文件不能被创建,请检查文件路径是否正确");
                    return;
                }
            }

            if (GUILayout.Button("删除脚本", GUILayout.Height(25)))
            {
                if (File.Exists(rm.path + @"\" + rm.ButtJoinHorUpdateScript + ".cs"))
                {
                    File.Delete(rm.path + @"\" + rm.ButtJoinHorUpdateScript + ".cs");
                    Debug.Log("脚本删除成功!!");
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}


/*

     */
