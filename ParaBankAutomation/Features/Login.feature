Feature: User Authentication
  As a ParaBank customer
  I want to be able to log in and log out
  So that I can access my banking account securely

  Background:
    Given I am on the ParaBank home page

  @smoke @regression
  Scenario: Successful login with valid credentials
    When I login with valid credentials
    Then I should see the account overview page
    And I should see a welcome message with the username

  @smoke @regression
  Scenario: Failed login with invalid credentials
    When I login with username "invaliduser" and password "wrongpass"
    Then I should see an error message "The username and password could not be verified."
    And I should remain on the login page

  @regression
  Scenario: Successful logout
    Given I am logged in as a valid user
    When I click on the logout link
    Then I should be redirected to the login page
    And I should see the login form

  @smoke
  Scenario Outline: Login with different invalid credential combinations
    When I login with username "<username>" and password "<password>"
    Then I should see an error message "The username and password could not be verified."
    
    Examples:
      | username    | password    |
      | john        | wrongpass   |
      | invaliduser | demo        |
      |             | demo        |
      | john        |             |