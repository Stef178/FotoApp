using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FotoApp.MVVM.Data;
using FotoApp.MVVM.Model;
using Microsoft.Maui.Controls;

namespace FotoApp.MVVM.View
{
    public partial class ManageAssignments : ContentPage
    {
        public ObservableCollection<Assignment> Assignments { get; set; } = new ObservableCollection<Assignment>();

        public ManageAssignments()
        {
            InitializeComponent();
            LoadAssignments();
            BindingContext = this;
        }

        private async void LoadAssignments()
        {
            var assignments = await App.Database.GetAllAsync<Assignment>();
            Assignments.Clear();
            foreach (var assignment in assignments)
            {
                Assignments.Add(assignment);
            }
        }

        private async void OnAddAssignment(object sender, EventArgs e)
        {
            string title = await DisplayPromptAsync("Nieuwe Opdracht", "Titel:");
            string description = await DisplayPromptAsync("Nieuwe Opdracht", "Beschrijving:");
            DateTime deadline = await DisplayAlert("Deadline instellen", "Wil je een deadline instellen?", "Ja", "Nee")
                                ? await DisplayDatePickerAsync()
                                : DateTime.Now;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
            {
                await DisplayAlert("Fout", "Titel en beschrijving zijn verplicht!", "OK");
                return;
            }

            var newAssignment = new Assignment { Title = title, Description = description, Deadline = deadline };
            await App.Database.AddAsync(newAssignment);
            LoadAssignments();
        }

        private async void OnDeleteAssignment(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int assignmentId)
            {
                bool confirm = await DisplayAlert("Bevestigen", "Weet je zeker dat je deze opdracht wilt verwijderen?", "Ja", "Nee");
                if (!confirm) return;

                var assignment = await App.Database.GetAsync<Assignment>(assignmentId);
                if (assignment != null)
                {
                    await App.Database.DeleteAsync(assignment);
                    LoadAssignments();
                }
            }
        }

        private async void OnEditAssignment(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int assignmentId)
            {
                var assignment = await App.Database.GetAsync<Assignment>(assignmentId);
                if (assignment == null) return;

                string newTitle = await DisplayPromptAsync("Opdracht Bewerken", "Nieuwe titel:", initialValue: assignment.Title);
                string newDescription = await DisplayPromptAsync("Opdracht Bewerken", "Nieuwe beschrijving:", initialValue: assignment.Description);
                DateTime newDeadline = await DisplayDatePickerAsync();

                if (!string.IsNullOrWhiteSpace(newTitle) && !string.IsNullOrWhiteSpace(newDescription))
                {
                    assignment.Title = newTitle;
                    assignment.Description = newDescription;
                    assignment.Deadline = newDeadline;
                    await App.Database.UpdateAsync(assignment);
                    LoadAssignments();
                }
            }
        }

        private async Task<DateTime> DisplayDatePickerAsync()
        {
            var result = await DisplayPromptAsync("Datum invoeren", "Voer een deadline in (yyyy-MM-dd):");
            return DateTime.TryParse(result, out DateTime selectedDate) ? selectedDate : DateTime.Now;
        }
    }
}
