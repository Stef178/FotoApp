using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FotoApp.MVVM.Data;
using FotoApp.MVVM.Model;
using Microsoft.Maui.Controls;

namespace FotoApp.MVVM.View
{
    public partial class ManageAssignments : ContentPage
    {
        public ObservableCollection<Assignment> Assignments { get; set; } = new ObservableCollection<Assignment>();
        public ObservableCollection<AssignmentTheme> Themes { get; set; } = new ObservableCollection<AssignmentTheme>();

        public ManageAssignments()
        {
            InitializeComponent();
            LoadAssignments();
            LoadThemes();
            BindingContext = this;
        }

        private async void LoadAssignments()
        {
            var assignments = await App.Database.GetAllAsync<Assignment>();
            var themes = await App.Database.GetAllAsync<AssignmentTheme>();

            Assignments.Clear();
            foreach (var assignment in assignments)
            {
                assignment.Theme = themes.FirstOrDefault(t => t.Id == assignment.ThemeId);
                Assignments.Add(assignment);
            }
        }

        private async void LoadThemes()
        {
            var themes = await App.Database.GetAllAsync<AssignmentTheme>();
            Themes.Clear();
            foreach (var theme in themes)
            {
                Themes.Add(theme);
            }
        }

        private async void OnAddAssignment(object sender, EventArgs e)
        {
            // Thema kiezen
            if (Themes.Count == 0)
            {
                await DisplayAlert("Fout", "Er zijn geen thema's beschikbaar!", "OK");
                return;
            }

            string selectedTheme = await DisplayActionSheet("Kies een thema", "Annuleer", null, Themes.Select(t => t.Name).ToArray());

            if (selectedTheme == "Annuleer" || selectedTheme == null)
                return;

            // Titel en beschrijving invoeren
            string title = await DisplayPromptAsync("Nieuwe Opdracht", "Titel:");
            string description = await DisplayPromptAsync("Nieuwe Opdracht", "Beschrijving:");

            string deadlineInput = await DisplayPromptAsync("Deadline instellen", "Voer de deadline in (aantal minuten):");
            int deadlineInMinutes = int.TryParse(deadlineInput, out int minutes) ? minutes : 0;

            var theme = Themes.FirstOrDefault(t => t.Name == selectedTheme);
            if (theme == null)
            {
                await DisplayAlert("Fout", "Het gekozen thema is niet gevonden!", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
            {
                await DisplayAlert("Fout", "Titel en beschrijving zijn verplicht!", "OK");
                return;
            }

            var newAssignment = new Assignment
            {
                Title = title,
                Description = description,
                DeadlineInMinutes = deadlineInMinutes,
                ThemeId = theme.Id
            };

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
                if (assignment != null)
                {
                    // Thema kiezen
                    string selectedTheme = await DisplayActionSheet("Kies een thema", "Annuleer", null, Themes.Select(t => t.Name).ToArray());

                    if (selectedTheme == "Annuleer" || selectedTheme == null)
                        return;

                    string newTitle = await DisplayPromptAsync("Bewerken", "Nieuwe titel:", initialValue: assignment.Title);
                    string newDescription = await DisplayPromptAsync("Bewerken", "Nieuwe beschrijving:", initialValue: assignment.Description);

                    string newDeadlineInput = await DisplayPromptAsync("Bewerken", "Nieuwe deadline (aantal minuten):", initialValue: assignment.DeadlineInMinutes.ToString());
                    int newDeadlineInMinutes = int.TryParse(newDeadlineInput, out int newMinutes) ? newMinutes : assignment.DeadlineInMinutes;

                    if (string.IsNullOrWhiteSpace(newTitle) || string.IsNullOrWhiteSpace(newDescription))
                    {
                        await DisplayAlert("Fout", "Titel en beschrijving zijn verplicht!", "OK");
                        return;
                    }

                    var theme = Themes.FirstOrDefault(t => t.Name == selectedTheme);
                    assignment.Title = newTitle;
                    assignment.Description = newDescription;
                    assignment.DeadlineInMinutes = newDeadlineInMinutes; // Bijwerken van de deadline
                    assignment.ThemeId = theme.Id; // Bijwerken van het thema

                    await App.Database.UpdateAsync(assignment);
                    LoadAssignments();
                }
            }
        }
    }
}
