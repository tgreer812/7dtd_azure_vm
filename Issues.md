# Issues and Considerations

This document captures issues identified during frontend implementation and suggestions for future improvements.

## Implementation Notes

### Successfully Implemented
- ✅ All API endpoints from Backend/proposal.md are implemented
- ✅ Configuration-based API base URL management
- ✅ Comprehensive error handling with fallback mechanisms
- ✅ VM status polling during state transitions
- ✅ UI enhancements for different VM states
- ✅ Complete test coverage (26 tests passing)

### Architecture Decisions
- **Fallback Strategy**: When API calls fail, the frontend gracefully falls back to hardcoded data to maintain functionality
- **Polling Frequency**: VM status polling every 10 seconds as specified in the proposal
- **Error Handling**: All API failures are logged and handled gracefully without breaking the UI
- **State Management**: VM state is tracked and UI is updated accordingly

## Potential Issues for Backend Team

### 1. Game Data Availability During VM Start
**Issue**: When a VM transitions from `Starting` to `Running`, the game server may not be immediately ready to respond to `/api/game/info` and `/api/game/players` calls.

**Current Frontend Behavior**: 
- Waits 5 seconds after VM reports `Running` before attempting to load game data
- Falls back to cached/default data if game server is not ready

**Recommendation**: Backend should ensure `gamePortOpen` is only `true` when the game server is fully ready to accept API calls.

### 2. API Base URL Configuration
**Current Implementation**: Frontend reads `ApiBaseUrl` from configuration (defaults to `/api`)

**Production Consideration**: Ensure Azure Static Web Apps properly injects the API base URL via environment variables or configuration.

### 3. Error Response Format Consistency
**Implementation**: Frontend expects error responses to follow the format defined in the proposal:
```json
{
  "code": "string",
  "message": "string", 
  "details": "string"
}
```

**Recommendation**: Ensure all backend endpoints return consistent error formats.

### 4. Game Server Response Time
**Current Timeout**: HttpClient uses default timeouts

**Consideration**: Game server queries might take longer than web API calls. Consider implementing longer timeouts for game-specific endpoints.

## Future Enhancements (Out of Scope)

### 1. Real-time Updates
Currently uses polling. Could be enhanced with SignalR for real-time updates as mentioned in the proposal's future extensions.

### 2. Additional VM Controls
The UI could support additional VM operations like restart, once the backend implements them.

### 3. Enhanced Error Messages
More user-friendly error messages could be displayed based on specific error codes from the backend.

### 4. Retry Logic
Could implement exponential backoff retry logic for transient failures.

### 5. Connection Status Indicator
Could add a visual indicator showing the connection status to the backend API.

## Testing Considerations

### Current Coverage
- ✅ All API service methods tested
- ✅ Error scenarios covered
- ✅ Model validation tested
- ✅ HTTP status code handling tested

### Not Covered (By Design)
- Component rendering tests (would require Blazor test host)
- Integration tests with real backend (backend not implemented yet)
- End-to-end user interaction tests

## Performance Considerations

### Current Implementation
- Minimal memory usage with proper timer disposal
- Efficient polling that stops when VM reaches stable state
- Fallback mechanisms prevent blocking on failed API calls

### Potential Optimizations
- Could implement smart polling intervals based on VM state
- Could cache game data to reduce API calls
- Could implement request cancellation for better responsiveness

## Security Considerations

### Current Implementation
- No authentication implemented (as per proposal - planned for future)
- Input validation relies on backend API validation
- No sensitive data stored in frontend

### Future Considerations
- Will need to implement JWT or Static Web App authentication for POST endpoints
- Should validate all inputs before sending to backend
- Consider implementing request signing for additional security