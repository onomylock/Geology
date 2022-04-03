﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Geology
{
    class MenuItemExtensions : DependencyObject
    {
           public static Dictionary<MenuItem, String> ElementToGroupNames = new Dictionary<MenuItem, String>();

           public static readonly DependencyProperty GroupNameProperty =
               DependencyProperty.RegisterAttached("GroupName",
                                            typeof(String),
                                            typeof(MenuItemExtensions),
                                            new PropertyMetadata(String.Empty, OnGroupNameChanged));

           public static void SetGroupName(MenuItem element, String value)
           {
                element.SetValue(GroupNameProperty, value);
           }

           public static String GetGroupName(MenuItem element)
           {
                return element.GetValue(GroupNameProperty).ToString();
           }

           private static void OnGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
           {
                //Add an entry to the group name collection
                var menuItem = d as MenuItem;

                if (menuItem != null)
                {
                     String newGroupName = e.NewValue.ToString();
                     String oldGroupName = e.OldValue.ToString();
                     if (String.IsNullOrEmpty(newGroupName))
                     {
                          //Removing the toggle button from grouping
                          RemoveCheckboxFromGrouping(menuItem);
                     }
                     else
                     {
                          //Switching to a new group
                          if (newGroupName != oldGroupName)
                          {
                              if (!String.IsNullOrEmpty(oldGroupName))
                              {
                                   //Remove the old group mapping
                                   RemoveCheckboxFromGrouping(menuItem);
                              }
                              ElementToGroupNames.Add(menuItem, e.NewValue.ToString());
                              menuItem.Unchecked += MenuItemUnChecked;
                              menuItem.Checked += MenuItemChecked;
                           //   menuItem.Unchecked += MenuItemUnChecked;
                          }
                     }
                }
           }

           private static void RemoveCheckboxFromGrouping(MenuItem checkBox)
           {
                ElementToGroupNames.Remove(checkBox);
                checkBox.Checked -= MenuItemChecked;
                checkBox.Unchecked -= MenuItemUnChecked;
           }

           static MenuItem menuItemUncheck;
           static void MenuItemChecked(object sender, RoutedEventArgs e)
           {
               var menuItem = e.OriginalSource as MenuItem;
               foreach (var item in ElementToGroupNames)
               {
                   if (item.Key != menuItem && item.Value == GetGroupName(menuItem))
                   {
                       menuItemUncheck = item.Key ;
                       item.Key.IsChecked = false;
                   }
               }
           }

           static void MenuItemUnChecked(object sender, RoutedEventArgs e)
           {
               var menuItem = e.OriginalSource as MenuItem;
               if (menuItemUncheck!=menuItem)
                   menuItem.IsChecked=true;
           }
    }
}
