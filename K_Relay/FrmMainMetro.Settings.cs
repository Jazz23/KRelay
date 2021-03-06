﻿using Lib_K_Relay.GameData;
using Lib_K_Relay.GameData.DataStructures;
using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Forms;
using System;
using System.Linq;
using System.Windows.Forms;

namespace K_Relay
{
    partial class FrmMainMetro
    {
        private FixedStyleManager m_themeManager;

        private void InitSettings()
        {
            Invoke((MethodInvoker)delegate
            {
                m_themeManager = new FixedStyleManager(this);
                themeCombobox.Items.AddRange(Enum.GetNames(typeof(MetroThemeStyle)));
                styleCombobox.Items.AddRange(Enum.GetNames(typeof(MetroColorStyle)));

                lstServers.Items.AddRange(GameData.Servers.Map.Select(x => x.Value.Name).OrderBy(x => x).ToArray());

                themeCombobox.SelectedValueChanged += themeCombobox_SelectedValueChanged;
                styleCombobox.SelectedValueChanged += styleCombobox_SelectedValueChanged;

                themeCombobox.SelectedItem = Config.Default.Theme.ToString();
                styleCombobox.SelectedItem = Config.Default.Style.ToString();

                tglStartByDefault.Checked = Config.Default.StartProxyByDefault;
                lstServers.SelectedItem = Config.Default.DefaultServerName;

                Config.Default.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "DefaultServerName")
                    {
                        string serverName = Config.Default.DefaultServerName;

                        // Update default server in Proxy class (used by State constructor)
                        Lib_K_Relay.Proxy.DefaultServer = GameData.Servers.ByName(serverName).Address;

                        Invoke((MethodInvoker)delegate
                        {
                            if (!lstServers.SelectedItem.Equals(serverName))
                            {
                                lstServers.SelectedItem = serverName;
                            }
                        });
                    }
                };

                m_themeManager.OnStyleChanged += m_themeManager_OnStyleChanged;
                m_themeManager_OnStyleChanged(null, null);
            });
        }

        private void styleCombobox_SelectedValueChanged(object sender, EventArgs e)
        {
            m_themeManager.Style = (MetroColorStyle)Enum.Parse(typeof(MetroColorStyle), (string)styleCombobox.SelectedItem, true);
        }

        private void themeCombobox_SelectedValueChanged(object sender, EventArgs e)
        {
            m_themeManager.Theme = (MetroThemeStyle)Enum.Parse(typeof(MetroThemeStyle), (string)themeCombobox.SelectedItem, true);
        }

        private void ChangeServer(ServerStructure server)
        {
            Config.Default.DefaultServerName = server.Name;
            Config.Default.Save();

            // Update the server address in all the states
            foreach (var state in _proxy.States)
            {
                state.Value.ConTargetAddress = server.Address;
            }
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            string oldServer = Config.Default.DefaultServerName;

            Config.Default.StartProxyByDefault = tglStartByDefault.Checked;
            Config.Default.DefaultServerName = lstServers.SelectedItem.ToString();
            Config.Default.Theme = (MetroThemeStyle)Enum.Parse(typeof(MetroThemeStyle), (string)themeCombobox.SelectedItem, true);
            Config.Default.Style = (MetroColorStyle)Enum.Parse(typeof(MetroColorStyle), (string)styleCombobox.SelectedItem, true);
            Config.Default.Save();

            if (Config.Default.DefaultServerName != oldServer)
            {
                // Get server by name
                ChangeServer(GameData.Servers.ByName(Config.Default.DefaultServerName));
            }

            MetroMessageBox.Show(this, "\nYour settings have been saved.", "Save Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private class FixedStyleManager
        {
            public event EventHandler OnThemeChanged;

            public event EventHandler OnStyleChanged;

            private MetroStyleManager m_manager;

            private MetroColorStyle m_colorStyle;
            private MetroThemeStyle m_themeStyle;

            public FixedStyleManager(MetroForm form)
            {
                m_manager = new MetroStyleManager(form.Container);
                m_manager.Owner = form;
            }

            public MetroColorStyle Style
            {
                get
                {
                    return m_colorStyle;
                }
                set
                {
                    m_colorStyle = value;
                    Update();
                    if (OnStyleChanged != null)
                    {
                        OnStyleChanged(this, new EventArgs());
                    }
                }
            }

            public MetroThemeStyle Theme
            {
                get
                {
                    return m_themeStyle;
                }
                set
                {
                    m_themeStyle = value;
                    Update();
                    if (OnThemeChanged != null)
                    {
                        OnThemeChanged(this, new EventArgs());
                    }
                }
            }

            public void Update()
            {
                (m_manager.Owner as MetroForm).Theme = m_themeStyle;
                (m_manager.Owner as MetroForm).Style = m_colorStyle;

                m_manager.Theme = m_themeStyle;
                m_manager.Style = m_colorStyle;
            }
        }
    }
}
