﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DeadDog.Audio.Library;

namespace DeadDog.Audio
{
    public class PlaylistSearcher<T>
    {
        private Playlist<T> playlist;

        public PlaylistSearcher(Playlist<T> playlist)
        {
            this.playlist = playlist;
        }

        public IEnumerable<PlaylistEntry<T>> Search(SearchMethods method, PredicateString<T> predicate, string searchstring)
        {
            return Search(method, predicate, searchstring.Trim().Split(' '));
        }
        public IEnumerable<PlaylistEntry<T>> Search(SearchMethods method, PredicateString<T> predicate, params string[] searchstring)
        {
            string[] search = searchstring;
            for (int i = 0; i < search.Length; i++)
                search[i] = search[i].ToLower();

            if (search.Length == 0)
            {
                foreach (PlaylistEntry<T> e in playlist)
                    yield return e;
            }

            for (int i = 0; i < playlist.Count; i++)
                if (test(playlist[i].Track, method, search, predicate))
                    yield return playlist[i];
        }

        private static bool test(T track, SearchMethods method, string[] searchstring, PredicateString<T> pre)
        {
            if (method == SearchMethods.ContainsAll)
            {
                for (int i = 0; i < searchstring.Length; i++)
                    if (!pre(track, searchstring[i]))
                        return false;
                return true;
            }
            else // method == SearchMethod.ContainsAny
            {
                for (int i = 0; i < searchstring.Length; i++)
                    if (pre(track, searchstring[i]))
                        return true;
                return false;
            }
        }

        public static bool ContainedInTitleArtistAlbum<T, L, R>(Track<T, L, R> track, string searchstring)
            where T : Track<T, L, R>
            where L : Album<T, L, R>
            where R : Artist<T, L, R>
        {
            return track.Contains(searchstring) || track.Album.Contains(searchstring) || track.Album.Artist.Contains(searchstring);
        }
        public static bool ContainedInTitleArtist<T, L, R>(Track<T, L, R> track, string searchstring)
            where T : Track<T, L, R>
            where L : Album<T, L, R>
            where R : Artist<T, L, R>
        {
            return track.Contains(searchstring) || track.Album.Artist.Contains(searchstring);
        }
        public static bool ContainedInTitle<T, L, R>(Track<T, L, R> track, string searchstring)
            where T : Track<T, L, R>
            where L : Album<T, L, R>
            where R : Artist<T, L, R>
        {
            return track.Contains(searchstring);
        }

        public static bool ContainedInTitleArtistAlbum(RawTrack track, string searchstring)
        {
            return (track.TrackTitle == null ? false : track.TrackTitle.ToLower().Contains(searchstring))
                || (track.AlbumTitle == null ? false : track.AlbumTitle.ToLower().Contains(searchstring))
                || (track.ArtistName == null ? false : track.ArtistName.ToLower().Contains(searchstring));
        }
        public static bool ContainedInTitleArtist(RawTrack track, string searchstring)
        {
            return (track.TrackTitle == null ? false : track.TrackTitle.ToLower().Contains(searchstring))
                || (track.ArtistName == null ? false : track.ArtistName.ToLower().Contains(searchstring));
        }
        public static bool ContainedInTitle(RawTrack track, string searchstring)
        {
            return track.TrackTitle == null ? false : track.TrackTitle.ToLower().Contains(searchstring);
        }
    }
}
