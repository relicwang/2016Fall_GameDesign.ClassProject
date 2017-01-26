using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GroupDecider))]
public class GroupDeciderInspector : Editor
{
    private static Type[] btTypes;
    private static string[] btTypeNames;

    static GroupDeciderInspector()
    {
        btTypes = ConcreteSubtypesOf(typeof(BehaviorTreeNode));
        btTypeNames = btTypes.Select(t => t.Name).ToArray();
    }

    private Vector2 scrollPosition;

    public override void OnInspectorGUI()
    {
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        var behaviorTreeNode = (GroupDecider)target;
        foldouts[behaviorTreeNode] = true;
        ShowTreeNode(behaviorTreeNode);
        EditorGUILayout.EndScrollView();
    }

    Dictionary<BehaviorTreeNode, bool> foldouts = new Dictionary<BehaviorTreeNode, bool>();

    private void ShowTreeNode(BehaviorTreeNode node)
    {
        EditorGUILayout.BeginHorizontal();
        if (!foldouts.ContainsKey(node))
            foldouts[node] = false;
        foldouts[node] = EditorGUILayout.Foldout(foldouts[node], node.GetType().Name);
        var newName = EditorGUILayout.TextField(node.name);
        if (newName != node.name)
        {
            Undo.RecordObject(node, "set name");
            node.name = newName;
        }
        EditorGUILayout.EndHorizontal();

        if (foldouts[node])
        {
            ShowFields(node);

            var d = node as GroupDecider;
            if (d)
                ShowChildren(d);
        }
    }

    private void ShowFields(BehaviorTreeNode node)
    {
        foreach (var f in node.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
            PropertyField(node, f);
    }

    private void ShowChildren(GroupDecider decider)
    {
        var selectedTypeIndex = EditorGUILayout.Popup("Children", -1, btTypeNames,
            GUILayout.ExpandWidth(false));
        EditorGUI.indentLevel += 1;
        for (int i = 0; i < decider.Children.Count; i++)
        {
            var child = decider.Children[i];
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(" ", GUILayout.Width(10));
            if (i > 0)
            {
                if (GUILayout.Button("Up", GUILayout.ExpandWidth(false)))
                {
                    Undo.RecordObject(decider, "Move child up");
                    var temp = decider.Children[i];
                    decider.Children[i] = decider.Children[i-1];
                    decider.Children[i - 1] = temp;
                }
            }
            if (i < decider.Children.Count - 1)
            {
                if (GUILayout.Button("Down", GUILayout.ExpandWidth(false)))
                {
                    Undo.RecordObject(decider, "Move child down");
                    var temp = decider.Children[i];
                    decider.Children[i] = decider.Children[i + 1];
                    decider.Children[i + 1] = temp;
                }
            }
            GUILayout.Label("   ");
            if (GUILayout.Button("Delete", GUILayout.ExpandWidth(false)))
            {
                Undo.RecordObject(decider, "Delete child");
                decider.Children.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
            ShowTreeNode(child);
        }
        EditorGUI.indentLevel -= 1;
        if (selectedTypeIndex >= 0)
        {
            Undo.RecordObject(decider, "add child");
            var added = (BehaviorTreeNode)CreateInstance(btTypes[selectedTypeIndex]);
            decider.Children.Add(added);
        }
    }

    static Type[] ConcreteSubtypesOf(Type type)
    {
        var children = new List<Type>();
        foreach (var t in type.Assembly.GetTypes())
        {
            if (!t.IsAbstract && t.IsSubclassOf(type))
                children.Add(t);
        }
        return children.ToArray();
    }

    void PropertyField(BehaviorTreeNode node, FieldInfo field)
    {
        if (field.FieldType.IsEnum)
        {
            var old = (Enum)field.GetValue(node);
            var current = EditorGUILayout.EnumPopup(field.Name, old);
            if (!old.Equals(current))
            {
                Undo.RecordObject(node, "set " + field.Name);
                field.SetValue(node, current);
            }
        }
        else
        {
            switch (field.FieldType.Name)
            {
                case "Int32":
                {
                    var old = (int) field.GetValue(node);
                    var current = EditorGUILayout.IntField(field.Name, old);
                    if (old != current)
                    {
                        Undo.RecordObject(node, "set " + field.Name);
                        field.SetValue(node, current);
                    }
                }
                    break;

                case "String":
                {
                    var old = (string) field.GetValue(node);
                    var current = EditorGUILayout.TextField(field.Name, old);
                    if (old != current)
                    {
                        Undo.RecordObject(node, "set " + field.Name);
                        field.SetValue(node, current);
                    }
                }
                    break;

                case "Single":
                {
                    var old = (float) field.GetValue(node);
                    var current = EditorGUILayout.FloatField(field.Name, old);
                    if (old != current)
                    {
                        Undo.RecordObject(node, "set " + field.Name);
                        field.SetValue(node, current);
                    }
                }
                    break;

                case "Boolean":
                {
                    var old = (bool) field.GetValue(node);
                    var current = EditorGUILayout.Toggle(field.Name, old);
                    if (old != current)
                    {
                        Undo.RecordObject(node, "set " + field.Name);
                        field.SetValue(node, current);
                    }
                }
                    break;
                default:
                    break;
            }
        }
    }
}
