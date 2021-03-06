<template>
  <v-layout row wrap>
    <v-flex xs12>
         <v-autocomplete
          :items="items"
          :search-input.sync="search"
          :loading="loadingEntry"
          v-model="model"
          :hide-no-data="(search === null || search === '' || search.length < 3) || loadingSearch"
          no-filter
          label="Search anime by title... (e.g. Death Note)"
          item-text="title"
          item-value="malId"
          :menu-props="{maxHeight:'300'}"
          prepend-icon="search"
        >
          <template v-slot:item="data">
            <template v-if="typeof data.item !== 'object'">
              <v-list-tile-content v-text="data.item"></v-list-tile-content>
            </template>
            <template v-else>
              <v-list-tile-avatar>
                <v-img class="dropdownAvatar" :src="pathToImage(data.item.imageUrl)" />
              </v-list-tile-avatar>
              <v-list-tile-content>
                <v-list-tile-title v-html="data.item.title"></v-list-tile-title>
              </v-list-tile-content>
            </template>
          </template>
        </v-autocomplete>
      <alert-ribbon 
        :alerts="alerts" />
    </v-flex>
  </v-layout>
</template>

<script>
import axios from 'axios'
import AlertRibbon from '@/components/shared/ui-components/AlertRibbon.vue'

export default {
  name: 'AnimeBrowser',
  components: {
    'alert-ribbon': AlertRibbon
  },
  props: {
    searchedId: {
      type: Array,
      required: false
    }
  },
  data () {
    return {
      entries: [],
      model: null,
      search: '',
      searchResults: [],
      loadingEntry: false,
      loadingSearch: false,
      timeout: null,
      timeoutLimit: 300,
      maximumAnimeNumber: 6,
      alerts: [
        {
          name: 'tooMuchRecords',
          text: 'You can choose 6 anime at max.',
          value: false
        },
        {
          name: 'alreadyOnTheList',
          text: 'This anime is already selected.',
          value: false
        },
        {
          name: 'noResultsFound',
          text: 'No results found!',
          value: false
        },
        {
          name: 'serviceUnavailable',
          text: 'The service is currently unavailable. Please come back later.',
          value: false
        }
      ]
    }
  },
  computed: {
    items () {
      return this.entries.map(entry => {
        return Object.assign({}, entry, { malId: entry.malId, title: entry.title, imageUrl: entry.imageUrl })
      })
    },
    cardInfoRequest () {
      return process.env.apiUrl +
            '/api/anime/' + String(this.model);
    }
  },
  methods: {
    sendAnimeRequest () {
      if (this.searchedId.length >= this.maximumAnimeNumber) {
        this.handleBrowsingError('tooMuchRecords')
      } else {
        this.loadingEntry = true
        if (this.searchedId.includes(parseInt(this.model))) {
          this.handleBrowsingError('alreadyOnTheList')
          this.model = null
          this.loadingEntry = false
        } else {
          axios.get(this.cardInfoRequest)
            .then((response) => {
              if (response.data !== null) {
                this.$emit('animeReturned', response.data)
                this.resetAlerts()
                this.resetSearch()
              }
            })
            .catch((error) => {
              this.handleBrowsingError('serviceUnavailable')
              this.resetErrorSearch(error)
            })
        }
      }
    },
    resetSearch() {
      this.loadingEntry = false;
      this.model = null;
      this.search = '';
    },
    resetErrorSearch(errorMessage) {
      console.log(errorMessage);
      this.resetSearch();
    },
    loadDataFromLink () {
      if (this.shareLinkData.length > 1 && this.shareLinkData.length < 6) {
        this.loading = true;
        this.shareLinkData.forEach(element => {
          if (this.searchedId.indexOf(element) === -1  && Number.parseInt(element) !== 'NaN' && Number.parseInt(element) > 0) {
            axios.get(process.env.apiUrl + '/api/anime/'+ String(element))
              .then((response) => {
                this.$emit('animeReturned', response.data);
                this.resetAlerts();
                this.resetSearch();
              })
              .catch((error) => {
                this.handleBrowsingError('serviceUnavailable');
                this.loading = false;
              })
          }
        })
      }
    },
    excludeFromSearchResults () {
      this.searchedId.forEach(id => {
        var index = this.entries.map(x => x.malId).indexOf(id);
        if (index > -1) {
          this.entries.splice(index, 1);
        }
      });
    },
    emitRunImmediately () {
      if (this.searchedId != null && this.shareLinkData != null) {
        if (this.searchedId.length === this.shareLinkData.length) {
          this.$emit('runImmediately');
        }
      }
    },
    handleBrowsingError (errorType) {
      var errorIndex = this.alerts.map(x => x.name).indexOf(errorType)
      if (errorIndex !== -1) {
        this.alerts[errorIndex].value = true;
      }
    },
    resetAlerts () {
      this.alerts.forEach(alert => {
        alert.value = false;
      });
    }
  },
  watch: {
    search (val) {
        clearTimeout(this.timeout)
        var self = this;
        this.timeout = setTimeout(function() {
          self.loadingSearch = true;

          if (self.model != null || val === '' || val === null || val.length < 3) {
            self.entries = [];
            return;
          }

          var requestUrl = process.env.apiUrl +
            '/api/anime/' +
            '?Title=' +
            String(val.replace('/', ' '));

          axios.get(requestUrl)
            .then(res => {
              if (res.data.results.length > 0) {
                self.entries = res.data.results;
                self.excludeFromSearchResults();
              }
              self.loadingSearch = false;
            })
            .catch(error => {
              console.log(error);
              this.handleBrowsingError('serviceUnavailable');
              self.loadingSearch = false;
            })
        }, this.timeoutLimit);
    },
    model (val) {
      if (val !== null)
        this.sendAnimeRequest();
    },
    searchedId: {
      handler: 'emitRunImmediately',
      immediate: true
    }
  },
  mounted () {
    if (this.$route.query !== null && !this.isEmpty(this.$route.query)) {
      if (this.$route.query.animeIds != null) {
        this.shareLinkData = this.$route.query.animeIds.split(';');
        this.loadDataFromLink();
      }
    }
  },
}
</script>
