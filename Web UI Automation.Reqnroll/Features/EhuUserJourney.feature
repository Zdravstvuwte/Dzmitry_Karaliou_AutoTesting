Feature: EHU public website end-to-end journey

  As a prospective student
  I want to explore key areas of the English EHU site
  So that I can find information about programs, contacts, and language options

  @e2e @smoke
  Scenario: Visitor explores About, search, contacts, and Lithuanian language
    Given the user opens the English home page
    When the user opens About from the home page
    Then the About page should be displayed with the expected heading
    When the user returns to the English home page
    And the user searches for "study programs"
    Then the search results should reflect the query "study programs"
    When the user opens the Contacts page
    Then the contacts page should list inquiry emails and phone numbers
    When the user returns to the English home page
    And the user switches the site language to Lithuanian
    Then the Lithuanian site version should be active
