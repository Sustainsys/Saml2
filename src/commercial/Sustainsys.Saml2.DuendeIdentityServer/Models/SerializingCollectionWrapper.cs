// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.DuendeIdentityServer.Models;

internal class SerializingCollectionWrapper(Client client) : ICollection<IndexedEndpoint>
{
    public int Count => client.RedirectUris.Count;

    public bool IsReadOnly => client.RedirectUris.IsReadOnly;

    public void Add(IndexedEndpoint item) => client.RedirectUris.Add(item.Serialize());
   
    public void Clear() => client.RedirectUris.Clear();
    public bool Contains(IndexedEndpoint item) => client.RedirectUris.Contains(item.Serialize());
    public void CopyTo(IndexedEndpoint[] array, int arrayIndex) => throw new NotImplementedException();
    public IEnumerator<IndexedEndpoint> GetEnumerator() => client.RedirectUris.Select(s => IndexedEndpoint.FromSerializedString(s)).GetEnumerator();
    public bool Remove(IndexedEndpoint item) => client.RedirectUris.Remove(item.Serialize());
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
