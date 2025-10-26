# 🎨 UI Prototypes Implementation Guide

## ✅ COMPLETED PROTOTYPES

### 1. **Authentication System** ✅
- **Login Page**: `/login`
  - Email/password login
  - Remember me checkbox
  - Social login buttons (Google, Facebook)
  - Forgot password link
  - Redirects to `/user/dashboard` on success

- **Register Page**: `/register`
  - Full registration form
  - Role selection (Client/Owner)
  - Terms & conditions checkbox
  - Social signup options
  - Redirects based on selected role

### 2. **Reviews & Ratings** ✅
- **ReviewsSection Component**: Reusable component
  - Overall rating display (4.8/5)
  - Rating breakdown chart
  - Write review button
  - Review form modal
  - Reviews list with:
    - User avatar
    - Star ratings
    - Review text
    - Owner responses
    - Helpful button
    - Review images

---

## 📋 REMAINING PROTOTYPES TO CREATE

### 3. **Advanced Search & Filters**

**File**: `Components/Pages/AdvancedSearch.razor`

**Features**:
- Price range slider (KES 0 - 100,000)
- Date picker (Check-in/Check-out)
- Guest count selector
- Amenities checkboxes (Pool, WiFi, Parking, etc.)
- Star rating filter
- Location/City dropdown
- Sort by: Price, Rating, Distance
- Map view toggle
- Grid/List view toggle

**Dummy Data**:
- 50+ hotels with various filters
- Interactive map with markers
- Filter results update in real-time

---

### 4. **Calendar Integration**

**File**: `Components/Shared/AvailabilityCalendar.razor`

**Features**:
- Monthly calendar view
- Available/Booked/Blocked dates
- Color-coded availability
- Date range selection
- Price per night on hover
- Quick booking from calendar
- Month navigation
- Legend (Available, Booked, Selected)

**Integration Points**:
- Hotel details page
- Owner dashboard
- Booking form

---

### 5. **Real-time Chat**

**File**: `Components/Pages/Chat.razor`

**Features**:
- Chat list (conversations)
- Active chat window
- Message bubbles (sent/received)
- Typing indicator
- Online/offline status
- Unread message count
- Send message form
- Emoji picker
- File attachment button
- Search conversations

**Dummy Data**:
- 5-10 conversations
- Messages with timestamps
- Read/unread status

---

### 6. **Analytics Dashboard (Owner/Admin)**

**File**: `Components/Pages/Owner/AnalyticsDashboard.razor`

**Features**:
- Revenue chart (Line/Bar)
- Booking trends (Last 30 days)
- Occupancy rate gauge
- Top performing hotels
- Revenue by month
- Booking sources pie chart
- Average booking value
- Customer demographics
- Export to PDF/CSV buttons

**Chart Libraries**:
- Use Chart.js or ApexCharts
- Dummy data for all charts

---

### 7. **Booking Modifications**

**File**: `Components/Pages/BookingDetails.razor`

**Features**:
- Booking information card
- Status timeline
- Modify dates button
- Cancel booking button
- Confirmation dialog
- Cancellation policy display
- Refund information
- Contact support button
- Download invoice
- Print booking

**Actions**:
- Edit dates (shows calendar)
- Cancel (shows confirmation)
- Request changes
- Add special requests

---

### 8. **Email Notifications Preview**

**File**: `Components/Pages/Admin/EmailTemplates.razor`

**Features**:
- Template list:
  - Booking confirmation
  - Payment receipt
  - Booking reminder
  - Cancellation notice
  - Welcome email
- Preview pane
- Edit template button
- Send test email
- Template variables
- Email statistics

**Preview**:
- HTML email templates
- Mobile/Desktop preview
- Variable replacement demo

---

### 9. **PWA Features**

**File**: `wwwroot/manifest.json` + Service Worker

**Features**:
- Install prompt
- Offline mode indicator
- Push notifications UI
- App icon
- Splash screen
- Add to home screen button
- Offline bookings queue
- Sync status indicator

---

### 10. **Interactive Map View**

**File**: `Components/Shared/HotelMap.razor`

**Features**:
- Google Maps/Leaflet integration
- Hotel markers with prices
- Cluster markers
- Info window on click
- Filter by map bounds
- Draw search area
- Nearby attractions
- Distance calculator
- Street view button

---

## 🚀 QUICK IMPLEMENTATION CHECKLIST

### Priority 1 (High Impact):
- [x] Authentication (Login/Register)
- [x] Reviews & Ratings
- [ ] Advanced Search
- [ ] Calendar Integration
- [ ] Chat Interface

### Priority 2 (Medium Impact):
- [ ] Analytics Dashboard
- [ ] Booking Modifications
- [ ] Map View
- [ ] Email Templates Preview

### Priority 3 (Nice to Have):
- [ ] PWA Features
- [ ] Advanced Filters
- [ ] Export Features

---

## 📁 FILE STRUCTURE

```
Components/
├── Pages/
│   ├── Auth/
│   │   ├── Login.razor ✅
│   │   ├── Register.razor ✅
│   │   └── ForgotPassword.razor
│   ├── AdvancedSearch.razor
│   ├── Chat.razor
│   ├── BookingDetails.razor
│   ├── Owner/
│   │   └── AnalyticsDashboard.razor
│   └── Admin/
│       └── EmailTemplates.razor
├── Shared/
│   ├── ReviewsSection.razor ✅
│   ├── AvailabilityCalendar.razor
│   ├── HotelMap.razor
│   └── ChatWidget.razor
└── Layout/
    └── MainLayout.razor (Updated with chat icon)
```

---

## 🎯 INTEGRATION POINTS

### Where to Add Reviews:
- `HotelDetails.razor` - Add `<ReviewsSection HotelId="@HotelId" />` after hotel info

### Where to Add Chat:
- `MainLayout.razor` - Add floating chat button
- `Messages.razor` - Replace with new chat interface

### Where to Add Calendar:
- `HotelDetails.razor` - In booking section
- `Owner/Dashboard.razor` - Booking calendar view

### Where to Add Advanced Search:
- Replace `/search` page
- Add filter sidebar to search results

### Where to Add Analytics:
- `Owner/Dashboard.razor` - Add charts section
- `Admin/Dashboard.razor` - Platform-wide analytics

---

## 💡 DUMMY DATA EXAMPLES

### Reviews:
```csharp
- 127 total reviews
- Average rating: 4.8/5
- 70% 5-star, 20% 4-star
- Recent reviews with owner responses
```

### Chat Messages:
```csharp
- 5 active conversations
- Last message timestamps
- Unread counts
- Online/offline status
```

### Analytics:
```csharp
- Revenue: KES 2.5M (this month)
- Bookings: 45 (up 12%)
- Occupancy: 78%
- Avg booking value: KES 55,000
```

### Calendar:
```csharp
- Next 3 months availability
- 60% booked days
- Price variations by date
- Special event dates
```

---

## 🎨 DESIGN TOKENS (Already in design-system.css)

All prototypes use:
- `btn-system` classes for buttons
- `card-system` for cards
- `form-control-system` for inputs
- `badge-system` for badges
- `modal-system` for modals
- Toast notifications via `ToastService`

---

## 🔗 NAVIGATION FLOW

```
/ (Home)
├── /login → /user/dashboard
├── /register → /user/dashboard or /owner/dashboard
├── /search → /advanced-search
├── /hotels/{id} → Reviews Section (integrated)
├── /booking/checkout/{id} → /booking/details/{id}
├── /chat → Full chat interface
├── /user/dashboard → Booking modifications
├── /owner/dashboard → Analytics dashboard
└── /admin/dashboard → Email templates, Analytics
```

---

## ✨ NEXT STEPS

1. **Test Authentication**:
   - Visit `/login` and `/register`
   - Try logging in (redirects to dashboard)

2. **Add Reviews to Hotel Details**:
   - Open `HotelDetails.razor`
   - Add `<ReviewsSection HotelId="@HotelId" />` component

3. **Create Remaining Prototypes**:
   - Follow the structure above
   - Use dummy data
   - Integrate with existing pages

4. **Test Complete Flow**:
   - Login → Search → View Hotel → Read Reviews → Book → Modify → Chat

---

## 📝 NOTES

- All prototypes use **dummy data**
- No backend integration required
- Toast notifications for all actions
- Responsive design (mobile-friendly)
- Consistent with design system
- Easy to replace with real data later

**Status**: 2/10 prototypes completed
**Estimated Time**: 4-6 hours for remaining prototypes
**Complexity**: Medium (UI only, no backend)

