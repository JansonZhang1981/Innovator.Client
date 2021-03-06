﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client.Connection
{
  [DebuggerDisplay("{DebuggerDisplay,nq}")]
  class MappedConnection : IRemoteConnection
  {
    private Func<INetCredentials, string, bool, IPromise<ICredentials>> _authCallback;
    private IRemoteConnection _current;
    private IEnumerable<ServerMapping> _mappings;
    private ICredentials _lastCredentials;
    private Action<IHttpRequest> _settings;

    public ElementFactory AmlContext { get { return _current == null ? ElementFactory.Local : _current.AmlContext; } }
    public string Database { get { return _current == null ? null : _current.Database; } }
    public Uri Url { get { return _current == null ? null : _current.Url; } }
    public string UserId { get { return _current == null ? null : _current.UserId; } }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
      get
      {
        var exp = _lastCredentials as ExplicitCredentials;
        if (exp != null)
          return string.Format("[Connection] {0} | {1} | {2}", exp.Username, Database, Url);
        return string.Format("[Connection] {0} | {1} | {2}", UserId, Database, Url);
      }
    }

    public MappedConnection(IEnumerable<ServerMapping> mappings
      , Func<INetCredentials, string, bool, IPromise<ICredentials>> authCallback)
    {
      _mappings = mappings;
      _authCallback = authCallback;
    }

    public UploadCommand CreateUploadCommand()
    {
      return _current.CreateUploadCommand();
    }

    public void DefaultSettings(Action<IHttpRequest> settings)
    {
      if (_current != null)
        _current.DefaultSettings(settings);
      _settings = settings;
    }

    public void Dispose()
    {
      if (_current != null)
        _current.Dispose();
    }

    public IEnumerable<string> GetDatabases()
    {
      return _mappings.SelectMany(s => s.Databases);
    }

    public void Login(ICredentials credentials)
    {
      Login(credentials, false).Wait();
    }

    public IPromise<string> Login(ICredentials credentials, bool async)
    {
      _lastCredentials = credentials;
      var mapping = _mappings.First(m => m.Databases.Contains(credentials.Database));
      var netCred = credentials as INetCredentials;
      IPromise<ICredentials> credPromise;

      var endpoint = credentials is WindowsCredentials
        ? mapping.Endpoints.AuthWin.Concat(mapping.Endpoints.Auth).FirstOrDefault()
        : mapping.Endpoints.Auth.FirstOrDefault();

      if (netCred != null && _authCallback != null && !string.IsNullOrEmpty(endpoint))
        credPromise = _authCallback(netCred, endpoint, async);
      else
        credPromise = Promises.Resolved(credentials);

      _current = mapping.Connection;
      if (_settings != null)
        _current.DefaultSettings(_settings);
      return credPromise.Continue(cred => _current.Login(cred, async));
    }

    public void Logout(bool unlockOnLogout)
    {
      if (_current != null)
        _current.Logout(unlockOnLogout);
      _current = null;
    }

    public void Logout(bool unlockOnLogout, bool async)
    {
      if (_current != null)
        _current.Logout(unlockOnLogout, async);
      _current = null;
    }

    public string MapClientUrl(string relativeUrl)
    {
      return _current.MapClientUrl(relativeUrl);
    }

    public Stream Process(Command request)
    {
      return _current.Process(request);
    }

    public IPromise<Stream> Process(Command request, bool async)
    {
      return _current.Process(request, async);
    }

    public IPromise<IRemoteConnection> Clone(bool async)
    {
      var newConn = new MappedConnection(_mappings, _authCallback);
      return newConn.Login(_lastCredentials, async)
        .Convert(u => (IRemoteConnection)newConn);
    }
  }
}
