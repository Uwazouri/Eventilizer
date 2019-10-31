///
///     Simple and lazy way to create events from typenames 
///     (free, open do-whatever-im-not-responsible-code, I don't care)
///     
/// HowToUse:
///     Eventilizer.Subscribe<TYPENAME>(Action<TYPENAME>)
///         - use to subscribe a callback to a typename event. 
///     Eventilizer.Unsubscribe<TYPENAME>(Action<TYPENAME>)
///         - I dont need to explain this.
///     Eventilizer.Trigger(TYPENAME)
///         - Less said is more.
/// 
/// Note:   Yes, I know it's a stupid name!
/// 
/// Note:   Cleanup your mess! Unsub before your callbacks go out or you'll get nulled.
/// 
/// Note:   Thread-safe by default, comment out '#define EVENTILISER_LOCK_THREADS' if you don't care.
/// 
/// Note:   Performance? well... haven't profiled it but sys-dics uses hashtable for lookup.
///         should be fine for largeish amount of events, maybe not so great for frame frequent calls.
///         what I mean is hash equals (generally) good scalability, that is: 
///             about same speed for 1 batch or 100000 (right?)
///         so maybe use something else for regular triggers since dics are not the fastest in
///         many other cases and if speed is an issue.
/// 
/// Note:   DON'T READ THE CODE! IT'S DISTUSTING!!!

#define EVENTILIZER_LOCK_THREADS

public class Eventilizer {
    private static object lockObject = new object();
    private static bool isInstantiated = false;
    private static Eventilizer instance = null;
    private static Eventilizer Instance() { if (isInstantiated) return instance; else { lock(lockObject) { if (!isInstantiated) instance = new Eventilizer(); isInstantiated = true; } return instance; } }
    private System.Collections.Generic.Dictionary<System.Type, System.Collections.Generic.List<object>> callbacks = new System.Collections.Generic.Dictionary<System.Type, System.Collections.Generic.List<object>>();
    public static void Subscribe<EVENT>(System.Action<EVENT> callback) {
#if EVENTILIZER_LOCK_THREADS
        lock (lockObject)
#endif
        { if (!Instance().callbacks.ContainsKey(typeof(EVENT))) Instance().callbacks.Add(typeof(EVENT), new System.Collections.Generic.List<object>());
          if (!Instance().callbacks[typeof(EVENT)].Contains(callback)) Instance().callbacks[typeof(EVENT)].Add(callback);}}
    public static void Unsubsrcibe<EVENT>(System.Action<EVENT> callback) {
#if EVENTILIZER_LOCK_THREADS
        lock (lockObject)
#endif
        { if (Instance().callbacks.ContainsKey(typeof(EVENT))) Instance().callbacks[typeof(EVENT)].Remove(callback); }}
    public static void Trigger<EVENT>(EVENT eventArgument) {
#if EVENTILIZER_LOCK_THREADS
        lock (lockObject)
#endif
        { if (Instance().callbacks.ContainsKey(typeof(EVENT))) foreach (object callback in Instance().callbacks[typeof(EVENT)]) (callback as System.Action<EVENT>).Invoke(eventArgument);}}
}
