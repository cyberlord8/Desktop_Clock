# Developer Backlog

## Planned

### Natural-language Timers and Alarms

Add a reminder feature that lets the user type a natural-language request instead of filling out a form with time controls and a description field.

Examples:

- “Set an alarm for 4 PM to remind me to feed the dogs.”
- “Remind me in 20 minutes to check the oven.”

Timers and Alarms are sibling concepts with different time semantics:

- A **Timer** counts down from a duration and fires when the duration elapses.
- An **Alarm** is scheduled for a calendar date and clock time, and may eventually support recurrence.

They should share reminder delivery, notification, dismissal, and snooze behavior without being represented as the same domain concept.

#### Initial scope

- Add persisted Timer and Alarm models to the core module.
- Add a natural-language input surface to the WPF application.
- Parse common duration and date/time phrases into a structured draft.
- Show the interpreted description, target, and type for confirmation before saving.
- Support one-shot Timers and one-shot Alarms first.
- Deliver reminders reliably when the widget is minimized or closed.
- Keep the parsing seam independent of the UI so a local parser can later be replaced or supplemented by an LLM adapter.

#### Acceptance criteria

- “In 20 minutes” creates a Timer draft.
- “At 4 PM” or “tomorrow at 8 AM” creates an Alarm draft.
- The user can review and confirm the interpreted request before it is scheduled.
- Ambiguous or incomplete requests are explained and are not silently scheduled.
- Timers and Alarms can be listed, cancelled, dismissed, and persisted across application restarts.
- Existing clock settings and behavior remain unaffected.

#### Follow-up scope

- Recurring Alarms.
- Pause/resume and restart behavior for Timers.
- Snooze options and missed-reminder handling.
- Richer natural-language coverage and optional LLM-backed parsing.
- Sound selection and notification customization.

### Stopwatch

Add an elapsed-time tool for measuring how long something has been happening, with start, pause, resume, reset, and lap support.

### Pomodoro and focus mode

Add a guided focus workflow built on Timers, with configurable work periods, short breaks, long breaks, and an optional cycle count.

### Personal routines

Let the user define repeatable, time-aware routines made from Timers, Alarms, calendar context, and reminder actions.

Examples:

- “Weekdays at 4 PM: remind me to feed the dogs.”
- “At 9:30 PM: start wind-down mode and remind me to prepare for bed.”
- “After my work Timer ends: start a 10-minute break.”

Routines should be explicit and user-controlled. The clock may suggest patterns later, but it must not silently create routines from observed behavior.

### Focus Shield

Add an intentional focus state that reduces interruptions while the user is working or concentrating.

#### Initial implementation direction

- Start with a clock-local shield that suppresses non-urgent clock-owned popup reminders and minimizes visual activity.
- Tie the shield to a Timer, Pomodoro work period, or manually selected duration.
- Show a quiet status indicator and the remaining focus time.
- Restore normal reminder behavior automatically when the focus period ends or the user exits the shield.
- Preserve urgent or explicitly escalated reminders according to the user's settings.

#### Follow-up scope

- Explore optional integration with Windows notification or focus controls.
- Decide whether calendar events should be allowed through during focus time.
- Add per-routine and per-reminder exceptions.

### Escalating reminders

Allow a reminder to become more noticeable over time when it has not been acknowledged.

#### Initial scope

- Support configurable escalation stages, such as popup, repeated popup, sound, and persistent overlay.
- Allow stages to repeat at configurable intervals until the reminder is acknowledged, dismissed, or snoozed.
- Let the user set a maximum number of attempts or a final escalation behavior.
- Respect quiet hours, Focus Shield, and per-reminder urgency settings.
- Record acknowledgement state so an escalation does not restart unexpectedly after an application restart.

Escalation should be opt-in and especially useful for reminders such as medication, cooking, pet care, or appointments. It must remain possible to dismiss or snooze an escalating reminder without disabling unrelated reminders.

### Timezone converter

Add a tool for converting a date and time between the configured clocks, for example, “What time is 9 AM Phoenix in Tokyo?” Account for daylight-saving rules and date changes.

### Meeting planner

Add a way to compare the local working or waking hours of multiple configured cities and identify reasonable overlapping meeting windows.

### World-clock improvements

Enhance clock readings with optional contextual information:

- UTC offset.
- Daylight-saving status or transition warning.
- Sunrise and sunset.
- Day/night indicator.

### Display customization

Add user-selectable presentation options:

- 12-hour or 24-hour time.
- Show or hide seconds.
- Configurable date format.
- Themes and accent colors.
- Widget opacity.
- Font size.

### Widget controls

Add controls for the desktop widget itself:

- Lock or unlock its position.
- Click-through mode.
- Keyboard shortcut to show or hide it.
- Per-monitor placement and display preferences.

### Windows integration

Integrate more naturally with Windows:

- System tray presence and menu.
- Start with Windows.
- Native reminder notifications.
- Configurable reminder sounds.

### Screen-saver mode

Add a full-screen presentation that displays the configured clocks while the user is away. It should reuse the core clock model and reading logic without changing widget mode behavior.

### Calendar context

Add optional calendar-oriented information:

- Weekday and week number.
- Local holidays.
- “Days until” events.

### Now / Next / Until

Create a unified time-awareness view that answers what is happening now, what is coming next, and how long remains until it happens.

#### Initial scope

- **Now:** show the current clock state, active Timer, current calendar event, or focus session.
- **Next:** identify the nearest upcoming Alarm, Timer completion, calendar event, or other configured time-based item.
- **Until:** show a live countdown to the selected upcoming item.
- Allow configurable local popup reminders before eligible items, such as 30 minutes, 15 minutes, 5 minutes, and at start.
- Allow reminder periods to be configured globally and overridden for an individual item where appropriate.
- Support dismiss, snooze, and quiet-hours behavior.
- Make the same information available to natural-language queries such as “What’s next?” and “How long until my meeting?”

#### Reminder behavior

Clock-owned popup reminders are opt-in and independently configurable. They may summarize read-only calendar data, but they must not modify provider calendars or silently replace the provider's own reminder settings.

### Read-only calendar awareness

Connect to Google Calendar and Microsoft 365/Outlook calendars in read-only mode so the clock can provide schedule context without becoming a calendar editor.

#### Initial scope

- Let the user connect a Google Calendar account or Microsoft account through the provider's sign-in flow.
- Let the user choose which calendars are visible.
- Show the next calendar event and a countdown in the clock's **Now / Next / Until** view.
- Show useful availability context such as “busy until 2:30 PM.”
- Make upcoming events available to natural-language queries such as “What’s next?” and “How long until my meeting?”
- Cache enough event data to remain useful when the network is temporarily unavailable.
- Preserve event time zones, all-day events, recurring events, cancellations, and private-event limitations.

#### Domain boundary

Calendar events remain read-only external schedule data. They are not Timers or Alarms. The clock may eventually offer to create a separate local Alarm before an event, but it must not silently duplicate the calendar provider's own reminders.

#### Explicitly out of scope

- Creating, editing, or deleting calendar events.
- Two-way synchronization.
- Attendee management or meeting invitations.
- Replacing the calendar provider's native reminder system.

### Voice and AI input

Expand natural-language interaction beyond creating Timers and Alarms. Potential requests include:

- “Set a timer for 25 minutes.”
- “What time is it in Berlin?”
- “Show me cities where it’s currently morning.”

Keep this behind a structured command interface so voice recognition or an LLM can be added without coupling the rest of the application to a provider.
