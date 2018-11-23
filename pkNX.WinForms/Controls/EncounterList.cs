﻿using System;
using System.Windows.Forms;
using pkNX.Sprites;
using pkNX.Structures;

namespace pkNX.WinForms
{
    public partial class EncounterList : UserControl
    {
        public EncounterList()
        {
            InitializeComponent();
        }

        public bool OverworldSpawn
        {
            set => NUD_Count.Visible = NUD_Duration.Visible = L_Count.Visible = L_Duration.Visible = value;
        }

        public void Initialize()
        {
            var dgvPicture = new DataGridViewImageColumn
            {
                HeaderText = "Sprite",
                DisplayIndex = 0,
                Width = 42,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            var dgvSpecies = new DataGridViewComboBoxColumn
            {
                HeaderText = "Species",
                DisplayIndex = 1,
                Width = 135,
                FlatStyle = FlatStyle.Flat
            };
            var dgvForm = new DataGridViewTextBoxColumn
            {
                HeaderText = "Form",
                DisplayIndex = 2,
                Width = 45,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            var dgvPercent = new DataGridViewTextBoxColumn
            {
                HeaderText = "Chance",
                DisplayIndex = 3,
                Width = 52,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            dgvSpecies.Items.AddRange(species);

            dgv.Columns.Add(dgvPicture);
            dgv.Columns.Add(dgvSpecies);
            dgv.Columns.Add(dgvForm);
            dgv.Columns.Add(dgvPercent);

            dgv.CellValueChanged += (s, e) =>
            {
                if (e.ColumnIndex == 0)
                    return;
                UpdateRowImage(e.RowIndex);
            };
        }

        private void UpdateRowImage(int row)
        {
            int sp = Array.IndexOf(species, dgv.Rows[row].Cells[1].Value);
            string formstr = (dgv.Rows[row].Cells[2].Value ?? 0).ToString();
            if (!int.TryParse(formstr, out var form) || (uint) form > 100)
                dgv.Rows[row].Cells[2].Value = 0;
            if (!int.TryParse(dgv.Rows[row].Cells[2].Value?.ToString(), out var rate) || (uint)rate > 100)
                dgv.Rows[row].Cells[3].Value = 0;

            dgv.Rows[row].Cells[0].Value = SpriteBuilder.GetSprite(sp, form, 0, 0, false, false);
        }

        private EncounterSlot[] Slots;
        public static string[] species;

        public void LoadSlots(EncounterSlot[] slots)
        {
            SaveCurrent();
            Slots = slots;

            dgv.Rows.Clear();
            dgv.Rows.Add(slots.Length);
            // Fill Entries
            for (int i = 0; i < slots.Length; i++)
            {
                var row = dgv.Rows[i];
                row.Cells[1].Value = species[slots[i].Species];
                row.Cells[2].Value = slots[i].Form;
                row.Cells[3].Value = slots[i].Probability;
                row.Height = 32;
            }

            dgv.CancelEdit();
        }

        public void SaveCurrent()
        {
            if (Slots == null)
                return;
            for (int i = 0; i < Slots.Length; i++)
            {
                SaveRow(i, Slots[i]);
            }
        }

        private void SaveRow(int row, EncounterSlot s)
        {
            int sp = Array.IndexOf(species, dgv.Rows[row].Cells[1].Value);
            string formstr = (dgv.Rows[row].Cells[2].Value ?? 0).ToString();
            int.TryParse(formstr, out var form);
            string probstr = (dgv.Rows[row].Cells[3].Value ?? 0).ToString();
            int.TryParse(probstr, out var prob);

            if (sp == 0)
            {
                s.Species = s.Form = s.Probability = 0;
                return;
            }

            s.Species = sp;
            s.Form = form;
            s.Probability = prob;
        }

        private void dgv_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgv.IsCurrentCellDirty)
                dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }
}
