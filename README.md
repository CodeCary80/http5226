# Itinerary Management System

## Project Overview
A Content Management System (CMS) designed to help organize and manage travel itineraries. The system allows users to create destinations, schedule activities, and manage member participations in various trips.

## Database Structure
- **Destinations**: Stores trip information (location, dates, budget)
- **Activities**: Events linked to destinations
- **Members**: Participant information
- **ActivityMembers**: Junction table managing activity participation

## Relationships
- One-to-Many: Destinations to Activities (one destination can have multiple activities)
- Many-to-Many: Members to Activities (managed through ActivityMembers)

## API Endpoints

### Destinations
```bash
# Get all destinations
curl http://localhost:5074/api/Destinations

# Get specific destination
curl http://localhost:5074/api/Destinations/1

# Create destination
curl -X POST http://localhost:5074/api/Destinations \
-H "Content-Type: application/json" \
-d '{
 "name": "Japan Trip",
 "location": "Tokyo",
 "startDate": "2024-07-01",
 "endDate": "2024-07-10",
 "description": "Summer trip",
 "budget": 5000
}'

# Update destination
curl -X PUT http://localhost:5074/api/Destinations/1 \
-H "Content-Type: application/json" \
-d '{
 "destinationId": 1,
 "name": "Updated Japan Trip",
 "location": "Tokyo",
 "startDate": "2024-07-01",
 "endDate": "2024-07-10",
 "description": "Updated trip",
 "budget": 6000
}'

# Delete destination
curl -X DELETE http://localhost:5074/api/Destinations/1

 ### Activities

# Get all activities
curl http://localhost:5074/api/Activities

# Get specific activity
curl http://localhost:5074/api/Activities/1

# Create activity
curl -X POST http://localhost:5074/api/Activities \
-H "Content-Type: application/json" \
-d '{
  "name": "Tokyo Tower Visit",
  "dateTime": "2024-07-02T14:00:00",
  "location": "Tokyo Tower",
  "description": "Visit tower",
  "cost": 1000,
  "destinationId": 1
}'

# Update activity
curl -X PUT http://localhost:5074/api/Activities/1 \
-H "Content-Type: application/json" \
-d '{
  "activityId": 1,
  "name": "Updated Tower Visit",
  "dateTime": "2024-07-02T14:00:00",
  "location": "Tokyo Tower",
  "description": "Evening visit",
  "cost": 1200,
  "destinationId": 1
}'

# Delete activity
curl -X DELETE http://localhost:5074/api/Activities/1


### Members
# Get all members
curl http://localhost:5074/api/Members

# Get specific member
curl http://localhost:5074/api/Members/1

# Create member
curl -X POST http://localhost:5074/api/Members \
-H "Content-Type: application/json" \
-d '{
  "name": "John Smith",
  "email": "john@email.com",
  "dietaryRestrictions": "None",
  "healthConsiderations": "None",
  "emergencyContact": "123-456-7890"
}'

# Update member
curl -X PUT http://localhost:5074/api/Members/1 \
-H "Content-Type: application/json" \
-d '{
  "memberId": 1,
  "name": "Updated John Smith",
  "email": "john.updated@email.com",
  "dietaryRestrictions": "None",
  "healthConsiderations": "None",
  "emergencyContact": "123-456-7890"
}'

# Delete member
curl -X DELETE http://localhost:5074/api/Members/1

### activity-member

# Get all activity-member relationships
curl http://localhost:5074/api/ActivityMembers

# Create relationship
curl -X POST http://localhost:5074/api/ActivityMembers \
-H "Content-Type: application/json" \
-d '{
  "activityId": 1,
  "memberId": 1,
  "isOrganizer": true,
  "notes": "Guide needed"
}'

# Delete relationship
curl -X DELETE http://localhost:5074/api/ActivityMembers/1/1