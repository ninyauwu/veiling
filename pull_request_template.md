1. Merging

    [ ] Did you select the dev branch?

2. File Structure

    [ ] Are assets like graphics, audio or fonts named using lower_snake_case (e.g., company_logo.png)?

3. Code Conventions

    [ ] Do private fields start with an underscore and use camelCase (e.g., _privateVariable)?
    [ ] Are static readonly variables and enum values named using UPPER_SNAKE_CASE?
    [ ] Are classes and public members named using PascalCase (e.g. PublicVariable)?
    [ ] Are function and variable names clear, concise, and descriptive without using unnecessary abbreviations?
    [ ] Are all public functions and fields documented with clear summaries where needed
    [ ] Does the code maintain a strict hierarchy, ensuring objects do not fetch references themselves or their parents in the hierarchy?
    [ ] Are references passed down through dependency injection as needed? Does each component only depend on its parent in the hierarchy?
    [ ] Have unnecessary/testing Debug.Logs or console.logs been removed?
    [ ] Has commented code been removed?

4. Proper Git Usage

    [ ] Does the branch name follow the convention of <type-of-change>/<change-made> in kebab-case (e.g. feature/user-dashboard)?
    [ ] Is each commit message formatted correctly according to the 7 rules outlined in https://cbea.ms/git-commit/?
