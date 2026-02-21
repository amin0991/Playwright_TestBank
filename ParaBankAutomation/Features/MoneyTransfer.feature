@MoneyTransfer
Feature: Inter-User Money Transfer
  As a ParaBank customer
  I want to send money to another user
  So that I can transfer funds between different accounts

  Background:
    Given I am on the ParaBank home page

  @Positive @Critical
  Scenario: Send money from john to highbrow90
    Given I am logged in as "john1"
    And I have at least one account with sufficient balance
    When I navigate to the bill pay page
    And I send 50 dollars to account of "highbrow90"
    Then I should see a bill payment confirmation
    And the payment should show the correct amount

  @Positive
  Scenario: Send money between my own accounts
    Given I am logged in as "john1"
    And I have multiple accounts
    When I navigate to the bill pay page
    And I send 25 dollars to my second account
    Then I should see a bill payment confirmation

  @Negative
  Scenario: Attempt to send money with insufficient funds
    Given I am logged in as "john1"
    When I navigate to the bill pay page
    And I attempt to send 999999 dollars to account of "highbrow90"
    Then I should see an error about the payment