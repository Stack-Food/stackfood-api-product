Feature: Create Product
    As a restaurant administrator
    I want to create new products
    So that customers can order them

Scenario: Create a valid product
    Given I have a valid category "Lanche"
    When I create a product with name "X-Burger" and price 25.90
    Then the product should be created successfully
    And the product should have name "X-Burger"
    And the product should have price 25.90
