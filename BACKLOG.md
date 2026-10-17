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
